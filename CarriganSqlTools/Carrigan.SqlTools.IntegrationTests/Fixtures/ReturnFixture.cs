//IGNORE SPELLING: localdb respawner dbo

using Carrigan.SqlTools.IntegrationTests.Models;
using Microsoft.Data.SqlClient;
using Respawn;

namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

public sealed class ReturnFixture : IAsyncLifetime
{
    private const string MasterConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=true;";

    private readonly string _dbName = "CarriganSqlToolsReturnDb_" + Guid.CreateVersion7().ToString("N");
    private Respawner? _respawner;

    public string ConnectionString { get; private set; } = string.Empty;

    public async ValueTask InitializeAsync()
    {
        ConnectionString = new SqlConnectionStringBuilder(MasterConnectionString)
        {
            InitialCatalog = _dbName
        }.ConnectionString;

        await using SqlConnection master = new(MasterConnectionString);
        await master.OpenAsync();

        using SqlCommand createDb = master.CreateCommand();
        createDb.CommandText = $"CREATE DATABASE [{_dbName}]";
        await createDb.ExecuteNonQueryAsync();

        master.ChangeDatabase(_dbName);

        using SqlCommand createTable = master.CreateCommand();
        createTable.CommandText = ReturnModel.CreateTableSql;
        await createTable.ExecuteNonQueryAsync();

        await using SqlConnection conn = new(ConnectionString);
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = ["dbo"],
            WithReseed = true
        });
    }

    public async Task ResetAsync()
    {
        if (_respawner == null)
            throw new InvalidOperationException("Respawner has not been initialized.");

        await using SqlConnection conn = new(ConnectionString);
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
    }

    public async ValueTask DisposeAsync()
    {
        await using SqlConnection master = new(MasterConnectionString);
        await master.OpenAsync();

        using SqlCommand dropDb = master.CreateCommand();
        dropDb.CommandText = $"""
            IF DB_ID(N'{_dbName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{_dbName}];
            END
            """;

        await dropDb.ExecuteNonQueryAsync();
    }
}