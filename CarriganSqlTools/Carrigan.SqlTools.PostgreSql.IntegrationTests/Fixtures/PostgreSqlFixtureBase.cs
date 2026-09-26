using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.SqlGenerators;
using Npgsql;
using Respawn;

//IGNORE SPELLING: respawner

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public abstract class PostgreSqlFixtureBase : IAsyncLifetime
{
    private const long DatabaseLifecycleLockKey = 0x434152524947414E; // "CARRIGAN"
    private const string SchemaName = "public";

    private static readonly PostgreSqlDialect Dialect = new();

    private readonly string MaintenanceConnectionString;
    private readonly string DatabaseName = "CarriganSqlToolsTestDb_" + Guid.CreateVersion7().ToString("N");
    private readonly IEnumerable<string> TableDefinitions;
    private readonly IEnumerable<SqlQuery> DatabaseSetups;

    private Respawner? _respawner;
    private bool _databaseCreated;

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

    private async Task<NpgsqlConnection> OpenDatabaseLifecycleConnectionAsync()
    {
        NpgsqlConnectionStringBuilder builder = new(MaintenanceConnectionString)
        {
            Pooling = false
        };

        NpgsqlConnection connection = new(builder.ConnectionString);

        try
        {
            await connection.OpenAsync();

            await using NpgsqlCommand lockCommand = connection.CreateCommand();
            lockCommand.CommandText = "SELECT pg_advisory_lock(@lockKey);";
            lockCommand.Parameters.AddWithValue("lockKey", DatabaseLifecycleLockKey);
            await lockCommand.ExecuteScalarAsync();

            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
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
        bool databaseCreationAttempted = false;

        try
        {
            await using (NpgsqlConnection maintenanceConnection = await OpenDatabaseLifecycleConnectionAsync())
            {
                string dbIdentifier = Dialect.QuoteIdentifier(DatabaseName);

                await using NpgsqlCommand createDb = maintenanceConnection.CreateCommand();
                createDb.CommandText = $"CREATE DATABASE {dbIdentifier};";
                databaseCreationAttempted = true;
                await createDb.ExecuteNonQueryAsync();
                _databaseCreated = true;
            }

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
        catch (Exception initializationException)
        {
            try
            {
                await DropDatabaseAsync(databaseCreationAttempted);
            }
            catch (Exception cleanupException)
            {
                throw new AggregateException
                (
                    $"Failed to initialize {DatabaseName}, and cleanup also failed.",
                    initializationException,
                    cleanupException
                );
            }

            throw;
        }
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
        await DropDatabaseAsync();
        GC.SuppressFinalize(this);
    }

    private async Task DropDatabaseAsync(bool attemptIfCreationFailed = false)
    {
        if (!_databaseCreated && !attemptIfCreationFailed)
            return;

        await using (NpgsqlConnection poolConnection = new(UnitTestConnectionString))
            NpgsqlConnection.ClearPool(poolConnection);

        await using NpgsqlConnection maintenanceConnection = await OpenDatabaseLifecycleConnectionAsync();

        string dbIdentifier = Dialect.QuoteIdentifier(DatabaseName);

        await using NpgsqlCommand dropDb = maintenanceConnection.CreateCommand();
        dropDb.CommandText =
            $"""
            DROP DATABASE IF EXISTS {dbIdentifier} WITH (FORCE);
            """;

        await dropDb.ExecuteNonQueryAsync();
        _databaseCreated = false;
    }
}
