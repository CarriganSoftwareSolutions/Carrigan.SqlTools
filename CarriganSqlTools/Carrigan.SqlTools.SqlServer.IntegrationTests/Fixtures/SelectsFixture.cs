//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class SelectsFixture : SqlFixtureBase
{
    public SelectsFixture()
        : base
        (
            [
                Book.CreateTableSqlServer,
                BookStats.CreateTableSqlServer
            ],

            Insert.BookInsertStatement
                .Concat(Insert.BookStatsInsertStatement)
        )
    {
    }
}