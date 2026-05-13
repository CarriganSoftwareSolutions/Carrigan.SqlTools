//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.IntegrationTests.Models;
using Microsoft.Data.SqlClient;
using Respawn;

namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

public sealed class FieldsFixture : IAsyncLifetime
{
    private const string MasterConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=true;";

    private readonly string _dbName = "CarriganSqlToolsTestDb_" + Guid.CreateVersion7().ToString("N");
    private Respawner? _respawner;

    public string ConnectionString { get; private set; } = string.Empty;

    public async ValueTask InitializeAsync()
    {
        ConnectionString = new SqlConnectionStringBuilder(MasterConnectionString)
        {
            InitialCatalog = _dbName
        }.ConnectionString;

        await using SqlConnection masterConnection = new(MasterConnectionString);
        await masterConnection.OpenAsync();

        using SqlCommand createDb = masterConnection.CreateCommand();
        createDb.CommandText = $"CREATE DATABASE [{_dbName}]";
        await createDb.ExecuteNonQueryAsync();

        masterConnection.ChangeDatabase(_dbName);

        using SqlCommand createTable = masterConnection.CreateCommand();
        createTable.CommandText = FieldsModel.CreateTableSql;
        await createTable.ExecuteNonQueryAsync();

        await using SqlConnection openForRespawn = new(ConnectionString);
        await openForRespawn.OpenAsync();

        RespawnerOptions options = new()
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = ["dbo"],
            WithReseed = true
        };

        _respawner = await Respawner.CreateAsync(openForRespawn, options);
    }

    /// <summary>Reset table rows to a pristine state between tests.</summary>
    public async Task ResetAsync()
    {
        if (_respawner == null)
            throw new InvalidOperationException("Respawner has not been initialized.");

        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    public async ValueTask DisposeAsync()
    {
        await using SqlConnection connection = new(MasterConnectionString);
        await connection.OpenAsync();

        using SqlCommand drop = connection.CreateCommand();
        drop.CommandText = $"""
            IF DB_ID(N'{_dbName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{_dbName}];
            END
            """;

        await drop.ExecuteNonQueryAsync();
    }
}