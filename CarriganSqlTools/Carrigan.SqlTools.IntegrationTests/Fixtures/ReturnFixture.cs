using Microsoft.Data.SqlClient;
using Respawn;
using Carrigan.SqlTools.IntegrationTests.Models;
//IGNORE SPELLING: localdb  respawner dbo
namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

public sealed class ReturnFixture : IAsyncLifetime
{
    public string ConnectionString { get; private set; } =
        "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=true;";

    private readonly string _dbName = "CarriganSqlToolsReturnDb_" + Guid.CreateVersion7().ToString("N");
    private Respawner? _respawner;

    public async Task InitializeAsync()
    {
        using SqlConnection master = new(ConnectionString);
        await master.OpenAsync();

        SqlCommand createDb = master.CreateCommand();
        createDb.CommandText = $"CREATE DATABASE [{_dbName}]";
        await createDb.ExecuteNonQueryAsync();

        ConnectionString += $";Initial Catalog={_dbName}";
        master.ChangeDatabase(_dbName);

        // Create ReturnModel table
        SqlCommand createTable = master.CreateCommand();
        createTable.CommandText = ReturnModel.CreateTableSql;
        await createTable.ExecuteNonQueryAsync();

        // Prepare respawner
        using SqlConnection conn = new(ConnectionString);
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
        using SqlConnection conn = new(ConnectionString);
        await conn.OpenAsync();
        await _respawner!.ResetAsync(conn);
    }

    public async Task DisposeAsync()
    {
        using SqlConnection master = new(ConnectionString.Replace($";Initial Catalog={_dbName}", ""));
        await master.OpenAsync();

        SqlCommand dropDb = master.CreateCommand();
        dropDb.CommandText = $@"
            ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            DROP DATABASE [{_dbName}];
        ";
        await dropDb.ExecuteNonQueryAsync();
    }
}
