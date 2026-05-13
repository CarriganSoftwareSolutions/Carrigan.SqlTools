using Carrigan.SqlTools.Dialects.PostgreSql;
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
    private readonly string TableDefinition;

    private Respawner? _respawner;

    protected PostgreSqlFixtureBase(string tableDefinition)
        : this(Configurations.MaintenanceDbConnectionString, tableDefinition)
    {
    }

    internal string TestsConnectionString
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

    protected PostgreSqlFixtureBase(string maintenanceConnectionString, string tableDefinition)
    {
        MaintenanceConnectionString = maintenanceConnectionString;
        TableDefinition = tableDefinition;
    }

    public async ValueTask InitializeAsync()
    {
        await using NpgsqlConnection maintenanceConnection = new(MaintenanceConnectionString);
        await maintenanceConnection.OpenAsync();

        string dbIdentifier = Dialect.QuoteIdentifier(DatabaseName);

        await using NpgsqlCommand createDb = maintenanceConnection.CreateCommand();
        createDb.CommandText = $"CREATE DATABASE {dbIdentifier};";
        await createDb.ExecuteNonQueryAsync();

        await using NpgsqlConnection testConnection = new(TestsConnectionString);
        await testConnection.OpenAsync();

        await using NpgsqlCommand createExtension = testConnection.CreateCommand();
        createExtension.CommandText = "CREATE EXTENSION IF NOT EXISTS pgcrypto;";
        await createExtension.ExecuteNonQueryAsync();

        await using NpgsqlCommand createTable = testConnection.CreateCommand();
        createTable.CommandText = TableDefinition;
        await createTable.ExecuteNonQueryAsync();

        _respawner = await Respawner.CreateAsync(testConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = [SchemaName],
            WithReseed = true
        });
    }

    public async Task ResetAsync()
    {
        if (_respawner == null)
            throw new InvalidOperationException($"{DatabaseName} Respawner has not been initialized.");

        await using NpgsqlConnection connection = new(TestsConnectionString);
        await connection.OpenAsync();

        await _respawner.ResetAsync(connection);
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