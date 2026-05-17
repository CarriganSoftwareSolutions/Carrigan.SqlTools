using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
using Microsoft.Data.SqlClient;
using Respawn;

// Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public abstract class SqlFixtureBase : IAsyncLifetime
{
    private const string SchemaName = "dbo";

    private static readonly SqlServerDialect Dialect = new();

    private readonly string MaintenanceConnectionString;
    private readonly string DatabaseName = "CarriganSqlToolsTestDb_" + Guid.CreateVersion7().ToString("N");
    private readonly IEnumerable<string> TableDefinitions;
    private readonly IEnumerable<SqlQuery> DatabaseSetups;

    private Respawner? _respawner;

    protected SqlFixtureBase(IEnumerable<string> tableDefinition)
    {
        MaintenanceConnectionString = Configurations.MaintenanceDbConnectionString;
        TableDefinitions = tableDefinition;
        DatabaseSetups = [];
    }
    protected SqlFixtureBase(IEnumerable<string> tableDefinition, IEnumerable<SqlQuery> databaseSetups)
    {
        MaintenanceConnectionString = Configurations.MaintenanceDbConnectionString;
        TableDefinitions = tableDefinition;
        DatabaseSetups = databaseSetups;
    }

    internal string UnitTestConnectionString
    {
        get
        {
            SqlConnectionStringBuilder builder = new(MaintenanceConnectionString)
            {
                InitialCatalog = DatabaseName
            };

            return builder.ConnectionString;
        }
    }
    private void ExecuteDatabaseSetups(SqlConnection connection)
    {
        foreach (SqlQuery query in DatabaseSetups)
        {
            Commands.ExecuteNonQuery(query, null, connection);
        }
    }

    public async ValueTask InitializeAsync()
    {
        string dbIdentifier = Dialect.QuoteIdentifier(DatabaseName);

        await using SqlConnection maintenanceConnection = new(MaintenanceConnectionString);
        await maintenanceConnection.OpenAsync();

        await using SqlCommand createDb = maintenanceConnection.CreateCommand();
        createDb.CommandText = $"CREATE DATABASE {dbIdentifier};";
        await createDb.ExecuteNonQueryAsync();

        await using SqlConnection unitTestConnection = new(UnitTestConnectionString);
        await unitTestConnection.OpenAsync();

        foreach(string tableDefinition in TableDefinitions)
        {
            await using SqlCommand createTable = unitTestConnection.CreateCommand();
            createTable.CommandText = tableDefinition;
            await createTable.ExecuteNonQueryAsync();
        }

        _respawner = await Respawner.CreateAsync(unitTestConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = [SchemaName],
            WithReseed = true
        });

        ExecuteDatabaseSetups(unitTestConnection);
    }

    public async Task ResetAsync()
    {
        if (_respawner == null)
            throw new InvalidOperationException($"{DatabaseName} Respawner has not been initialized.");

        await using SqlConnection unitTestConnection = new(UnitTestConnectionString);
        await unitTestConnection.OpenAsync();

        await _respawner.ResetAsync(unitTestConnection);

        ExecuteDatabaseSetups(unitTestConnection);
    }

    public async ValueTask DisposeAsync()
    {
        string dbIdentifier = Dialect.QuoteIdentifier(DatabaseName);
        string dbNameLiteral = DatabaseName.Replace("'", "''");

        await using SqlConnection maintenanceConnection = new(MaintenanceConnectionString);
        await maintenanceConnection.OpenAsync();

        await using SqlCommand dropDb = maintenanceConnection.CreateCommand();
        dropDb.CommandText = $"""
            IF DB_ID(N'{dbNameLiteral}') IS NOT NULL
            BEGIN
                ALTER DATABASE {dbIdentifier} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE {dbIdentifier};
            END
            """;

        await dropDb.ExecuteNonQueryAsync();

        GC.SuppressFinalize(this);
    }
}