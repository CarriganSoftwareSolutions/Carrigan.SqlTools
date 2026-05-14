using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.IntegrationTests;
using Microsoft.Data.SqlClient;
using Respawn;
using Xunit;

// Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

public abstract class SqlFixtureBase : IAsyncLifetime
{
    private const string SchemaName = "dbo";

    private static readonly SqlServerDialect Dialect = new();

    private readonly string MaintenanceConnectionString;
    private readonly string DatabaseName = "CarriganSqlToolsTestDb_" + Guid.CreateVersion7().ToString("N");
    private readonly string TableDefinition;

    private Respawner? _respawner;

    protected SqlFixtureBase(string tableDefinition)
        : this(Configurations.MaintenanceDbConnectionString, tableDefinition)
    {
    }

    protected SqlFixtureBase(string maintenanceConnectionString, string tableDefinition)
    {
        MaintenanceConnectionString = maintenanceConnectionString;
        TableDefinition = tableDefinition;
    }

    internal string UnitTestsConnectionString
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

    public async ValueTask InitializeAsync()
    {
        string dbIdentifier = Dialect.QuoteIdentifier(DatabaseName);

        await using SqlConnection maintenanceConnection = new(MaintenanceConnectionString);
        await maintenanceConnection.OpenAsync();

        await using SqlCommand createDb = maintenanceConnection.CreateCommand();
        createDb.CommandText = $"CREATE DATABASE {dbIdentifier};";
        await createDb.ExecuteNonQueryAsync();

        await using SqlConnection unitTestConnection = new(UnitTestsConnectionString);
        await unitTestConnection.OpenAsync();

        await using SqlCommand createTable = unitTestConnection.CreateCommand();
        createTable.CommandText = TableDefinition;
        await createTable.ExecuteNonQueryAsync();

        _respawner = await Respawner.CreateAsync(unitTestConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = [SchemaName],
            WithReseed = true
        });
    }

    public async Task ResetAsync()
    {
        if (_respawner == null)
            throw new InvalidOperationException($"{DatabaseName} Respawner has not been initialized.");

        await using SqlConnection connection = new(UnitTestsConnectionString);
        await connection.OpenAsync();

        await _respawner.ResetAsync(connection);
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