using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Npgsql;
using Respawn;
using Xunit;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public abstract class PostgreSqlFixtureBase : IAsyncLifetime
{
    private const string SchemaName = "public";

    private static readonly PostgreSqlDialect Dialect = new();

    private readonly string MaintenanceConnectionString;
    private readonly string DatabaseName = "CarriganSqlToolsTestDb_" + Guid.CreateVersion7().ToString("N");
    private readonly IEnumerable<string> TableDefinitions;
    private readonly IEnumerable<SqlQuery> DatabaseSetups;

    private Respawner? _respawner;

    internal string UnitTestConnectionString
    {
        get
        {
            NpgsqlConnectionStringBuilder builder = new(MaintenanceConnectionString)
            {
                Database = DatabaseName
            };

            return builder.ConnectionString;
        }
    }
    private void ExecuteDatabaseSetups(NpgsqlConnection connection)
    {
        foreach (SqlQuery query in DatabaseSetups)
        {
            Commands.ExecuteNonQuery(query, null, connection);
        }
    }

    protected PostgreSqlFixtureBase(IEnumerable<string> tableDefinition)
    {
        MaintenanceConnectionString = Configurations.MaintenanceDbConnectionString;
        TableDefinitions = tableDefinition;
        DatabaseSetups = [];
    }

    protected PostgreSqlFixtureBase(IEnumerable<string> tableDefinition, IEnumerable<SqlQuery> databaseSetups)
    {
        MaintenanceConnectionString = Configurations.MaintenanceDbConnectionString;
        TableDefinitions = tableDefinition;
        DatabaseSetups = databaseSetups;
    }

    public async ValueTask InitializeAsync()
    {
        await using NpgsqlConnection maintenanceConnection = new(MaintenanceConnectionString);
        await maintenanceConnection.OpenAsync();

        string dbIdentifier = Dialect.QuoteIdentifier(DatabaseName);

        await using NpgsqlCommand createDb = maintenanceConnection.CreateCommand();
        createDb.CommandText = $"CREATE DATABASE {dbIdentifier};";
        await createDb.ExecuteNonQueryAsync();

        await using NpgsqlConnection unitTestConnection = new(UnitTestConnectionString);
        await unitTestConnection.OpenAsync();

        await using NpgsqlCommand createExtension = unitTestConnection.CreateCommand();
        createExtension.CommandText = "CREATE EXTENSION IF NOT EXISTS pgcrypto;";
        await createExtension.ExecuteNonQueryAsync();

        foreach (string tableDefinition in TableDefinitions)
        {
            await using NpgsqlCommand createTable = unitTestConnection.CreateCommand();
            createTable.CommandText = tableDefinition;
            await createTable.ExecuteNonQueryAsync();
        }

        _respawner = await Respawner.CreateAsync(unitTestConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = [SchemaName],
            WithReseed = true
        });

        ExecuteDatabaseSetups(unitTestConnection);
    }

    public async Task ResetAsync()
    {
        if (_respawner == null)
            throw new InvalidOperationException($"{DatabaseName} Respawner has not been initialized.");

        await using NpgsqlConnection unitTestConnection = new(UnitTestConnectionString);
        await unitTestConnection.OpenAsync();

        await _respawner.ResetAsync(unitTestConnection);

        ExecuteDatabaseSetups(unitTestConnection);
    }

    public async ValueTask DisposeAsync()
    {
        await using NpgsqlConnection maintenanceConnection = new(MaintenanceConnectionString);
        await maintenanceConnection.OpenAsync();

        string dbIdentifier = Dialect.QuoteIdentifier(DatabaseName);

        await using NpgsqlCommand dropDb = maintenanceConnection.CreateCommand();
        dropDb.CommandText =
            $"""
            DROP DATABASE IF EXISTS {dbIdentifier} WITH (FORCE);
            """;

        await dropDb.ExecuteNonQueryAsync();

        GC.SuppressFinalize(this);
    }
}