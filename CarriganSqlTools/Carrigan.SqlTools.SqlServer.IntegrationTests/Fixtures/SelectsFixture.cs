//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.DataSets;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class SelectsFixture : SqlFixtureBase
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