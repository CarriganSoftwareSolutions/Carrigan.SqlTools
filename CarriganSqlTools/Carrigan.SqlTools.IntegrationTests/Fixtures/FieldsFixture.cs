//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.IntegrationTests.Models;
using Microsoft.Data.SqlClient;
using Respawn;

namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

public sealed class FieldsFixture : IAsyncLifetime
{
    private const string MaintenanceConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=true;";

    private readonly string _dbName = "CarriganSqlToolsTestDb_" + Guid.CreateVersion7().ToString("N");
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
            await using SqlConnection masterConnection = new(MaintenanceConnectionString);
            await masterConnection.OpenAsync();

            await using SqlCommand createDb = masterConnection.CreateCommand();
            createDb.CommandText = $"CREATE DATABASE [{_dbName}]";
            await createDb.ExecuteNonQueryAsync();
            _databaseCreated = true;

            await using SqlConnection unitTestConnection = new(ConnectionString);
            await unitTestConnection.OpenAsync();

            await using SqlCommand createTable = unitTestConnection.CreateCommand();
            createTable.CommandText = FieldsModel.CreateTableSql;
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

    /// <summary>Reset table rows to a pristine state between tests.</summary>
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

        await using SqlConnection connection = new(MaintenanceConnectionString);
        await connection.OpenAsync();

        await using SqlCommand drop = connection.CreateCommand();
        drop.CommandText = $"""
            IF DB_ID(N'{dbNameLiteral}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{_dbName}];
            END
            """;
        await drop.ExecuteNonQueryAsync();
        _databaseCreated = false;
    }
}
