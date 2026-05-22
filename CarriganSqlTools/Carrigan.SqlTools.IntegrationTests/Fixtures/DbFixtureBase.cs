using Carrigan.SqlTools.SqlGenerators;
using Microsoft.Testing.Platform.Configurations;
using Respawn;
using System.Data.Common;

// Ignore Spelling: Respawn, Respawner, Reseed, Carrigan, SqlTools

namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

public abstract class DbFixtureBase<TConnection> : IAsyncLifetime where TConnection : DbConnection
{
    private const string CarriganDBPrefix = "CarriganSqlToolsTestDb_";
    private readonly string MaintenanceConnectionString;
    private readonly IReadOnlyCollection<string> TableDefinitions;
    private readonly IReadOnlyCollection<SqlQuery> DatabaseSetups;

    private Respawner? _respawner;

    protected DbFixtureBase(string maintenanceConnectionString, IEnumerable<string> tableDefinitions) :
        this(maintenanceConnectionString, tableDefinitions, [])
    {
    }

    protected DbFixtureBase(string maintenanceConnectionString, IEnumerable<string> tableDefinitions, IEnumerable<SqlQuery> databaseSetups)
    {
        MaintenanceConnectionString = maintenanceConnectionString;
        TableDefinitions = [.. tableDefinitions];
        DatabaseSetups = [.. databaseSetups];
    }

    public string UnitTestConnectionString => BuildUnitTestConnectionString(MaintenanceConnectionString, TestDatabaseName);

    protected string TestDatabaseName { get; } = $"{CarriganDBPrefix}{Guid.CreateVersion7():N}";

    protected abstract IDbAdapter DbAdapter { get; }

    protected abstract string SchemaName { get; }

    protected abstract TConnection CreateConnection(string connectionString);

    protected abstract string BuildUnitTestConnectionString(string maintenanceConnectionString, string databaseName);

    protected abstract string CreateDatabaseCommandText(string databaseName);

    protected abstract string DropDatabaseCommandText(string databaseName);

    protected abstract void ExecuteDatabaseSetup(SqlQuery query, TConnection connection);


    protected virtual Task BeforeCreateTablesAsync(TConnection connection) => Task.CompletedTask;

    public async ValueTask InitializeAsync()
    {
        await using TConnection maintenanceConnection = CreateConnection(MaintenanceConnectionString);
        await maintenanceConnection.OpenAsync();

        await ExecuteCommandAsync(maintenanceConnection, CreateDatabaseCommandText(TestDatabaseName));

        await using TConnection unitTestConnection = CreateConnection(UnitTestConnectionString);
        await unitTestConnection.OpenAsync();

        await BeforeCreateTablesAsync(unitTestConnection);

        foreach (string tableDefinition in TableDefinitions)
        {
            await ExecuteCommandAsync(unitTestConnection, tableDefinition);
        }

        _respawner = await Respawner.CreateAsync(unitTestConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter,
            SchemasToInclude = [SchemaName],
            WithReseed = true
        });

        ExecuteDatabaseSetups(unitTestConnection);
    }

    public async Task ResetAsync()
    {
        if (_respawner == null)
            throw new InvalidOperationException($"{TestDatabaseName} Respawner has not been initialized.");

        await using TConnection unitTestConnection = CreateConnection(UnitTestConnectionString);
        await unitTestConnection.OpenAsync();

        await _respawner.ResetAsync(unitTestConnection);

        ExecuteDatabaseSetups(unitTestConnection);
    }

    public async ValueTask DisposeAsync()
    {
        await using TConnection maintenanceConnection = CreateConnection(MaintenanceConnectionString);
        await maintenanceConnection.OpenAsync();

        await ExecuteCommandAsync(maintenanceConnection, DropDatabaseCommandText(TestDatabaseName));

        GC.SuppressFinalize(this);
    }

    protected static async Task ExecuteCommandAsync(TConnection connection, string commandText)
    {
        await using DbCommand command = connection.CreateCommand();
        command.CommandText = commandText;
        await command.ExecuteNonQueryAsync();
    }

    private void ExecuteDatabaseSetups(TConnection connection)
    {
        foreach (SqlQuery query in DatabaseSetups)
        {
            ExecuteDatabaseSetup(query, connection);
        }
    }
}