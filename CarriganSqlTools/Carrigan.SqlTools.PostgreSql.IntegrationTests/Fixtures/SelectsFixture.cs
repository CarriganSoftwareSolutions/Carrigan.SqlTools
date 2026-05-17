//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.PostgreSql.IntegrationTests.DataSets;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Respawn;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class SelectsFixture : PostgreSqlFixtureBase
{
    public SelectsFixture()
        : base
        (
            [
                Book.CreateTableSql,
                BookStats.CreateTableSql
            ],

            BookDataSet.InsertStatement
                .Concat(BookStatsDataSet.InsertStatement)
        )
    {
    }
}