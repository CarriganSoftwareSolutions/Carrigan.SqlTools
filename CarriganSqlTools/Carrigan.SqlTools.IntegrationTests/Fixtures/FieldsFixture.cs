//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Microsoft.Data.SqlClient;
using Respawn;
using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

public sealed class FieldsFixture : IAsyncLifetime
{
    public string ConnectionString { get; private set; } =
        "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=true;";

    private readonly string _dbName = "CarriganSqlToolsTestDb_" + Guid.CreateVersion7().ToString("N");
    private Respawner? _respawner;

    public async Task InitializeAsync()
    {
        // Create database
        SqlConnection masterConnection = new(ConnectionString);
        await masterConnection.OpenAsync();

        SqlCommand createDb = masterConnection.CreateCommand();
        createDb.CommandText = $"CREATE DATABASE [{_dbName}]";
        await createDb.ExecuteNonQueryAsync();
        createDb.Dispose();

        // Point to new DB and create schema from model
        ConnectionString += $";Initial Catalog={_dbName}";
        masterConnection.ChangeDatabase(_dbName);

        SqlCommand createTable = masterConnection.CreateCommand();
        createTable.CommandText = FieldsModel.CreateTableSql;
        await createTable.ExecuteNonQueryAsync();
        createTable.Dispose();

        masterConnection.Dispose();

        // Prepare Respawner for fast resets
        SqlConnection openForRespawn = new(ConnectionString);
        await openForRespawn.OpenAsync();

        RespawnerOptions options = new()
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = ["dbo"],
            WithReseed = true
        };

        _respawner = await Respawner.CreateAsync(openForRespawn, options);
        openForRespawn.Dispose();
    }

    /// <summary>Reset table rows to a pristine state between tests.</summary>
    public async Task ResetAsync()
    {
        if (_respawner == null)
            throw new InvalidOperationException("Respawner has not been initialized.");

        SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
        connection.Dispose();
    }

    public async Task DisposeAsync()
    {
        // Drop the test database after all tests in this class complete
        string masterCs = ConnectionString.Replace($";Initial Catalog={_dbName}", string.Empty);
        SqlConnection connection = new(masterCs);
        await connection.OpenAsync();

        SqlCommand drop = connection.CreateCommand();
        drop.CommandText = $"""
            ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            DROP DATABASE [{_dbName}];
            """;
        await drop.ExecuteNonQueryAsync();

        drop.Dispose();
        connection.Dispose();
    }
}
