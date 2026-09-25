using Carrigan.SqlTools.IntegrationTests.Models;
using Microsoft.Data.SqlClient;
using Respawn;

//IGNORE SPELLING: localdb respawner dbo
namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

public sealed class ReturnFixture : IAsyncLifetime
{
    private const string MaintenanceConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=true;";

    private readonly string _dbName = "CarriganSqlToolsReturnDb_" + Guid.CreateVersion7().ToString("N");
    private Respawner? _respawner;
    private bool _databaseCreated;

    public string ConnectionString =>
        new SqlConnectionStringBuilder(MaintenanceConnectionString)
        {
            InitialCatalog = _dbName
        }.ConnectionString;

    public async ValueTask InitializeAsync()
    {
        try
        {
            await using SqlConnection master = new(MaintenanceConnectionString);
            await master.OpenAsync();

            await using SqlCommand createDb = master.CreateCommand();
            createDb.CommandText = $"CREATE DATABASE [{_dbName}]";
            await createDb.ExecuteNonQueryAsync();
            _databaseCreated = true;

            await using SqlConnection unitTestConnection = new(ConnectionString);
            await unitTestConnection.OpenAsync();

            await using SqlCommand createTable = unitTestConnection.CreateCommand();
            createTable.CommandText = ReturnModel.CreateTableSql;
            await createTable.ExecuteNonQueryAsync();

            _respawner = await Respawner.CreateAsync(unitTestConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                SchemasToInclude = ["dbo"],
                WithReseed = true
            });
        }
        catch (Exception initializationException)
        {
            try
            {
                await DropDatabaseAsync();
            }
            catch (Exception cleanupException)
            {
                throw new AggregateException
                (
                    $"Failed to initialize {_dbName}, and cleanup also failed.",
                    initializationException,
                    cleanupException
                );
            }

            throw;
        }
    }

    public async Task ResetAsync()
    {
        if (_respawner is null)
            throw new InvalidOperationException("Respawner has not been initialized.");

        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    public async ValueTask DisposeAsync()
    {
        await DropDatabaseAsync();
        GC.SuppressFinalize(this);
    }

    private async Task DropDatabaseAsync()
    {
        if (!_databaseCreated)
            return;

        using (SqlConnection poolConnection = new(ConnectionString))
            SqlConnection.ClearPool(poolConnection);

        string dbNameLiteral = _dbName.Replace("'", "''");

        await using SqlConnection master = new(MaintenanceConnectionString);
        await master.OpenAsync();

        await using SqlCommand dropDb = master.CreateCommand();
        dropDb.CommandText = $"""
            IF DB_ID(N'{dbNameLiteral}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{_dbName}];
            END
            """;
        await dropDb.ExecuteNonQueryAsync();
        _databaseCreated = false;
    }
}
