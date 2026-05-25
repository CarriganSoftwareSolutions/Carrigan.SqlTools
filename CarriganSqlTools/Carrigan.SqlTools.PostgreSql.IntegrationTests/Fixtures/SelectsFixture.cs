//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Inserts;
using Respawn;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class SelectsFixture : PostgreSqlFixtureBase
{
    public SelectsFixture()
        : base
        (
            [
                Book.CreateTablePostgreSql,
                BookStats.CreateTablePostgreSql
            ],

            Insert.BookInsertStatement
                .Concat(Insert.BookStatsInsertStatement)
        )
    {
    }
}