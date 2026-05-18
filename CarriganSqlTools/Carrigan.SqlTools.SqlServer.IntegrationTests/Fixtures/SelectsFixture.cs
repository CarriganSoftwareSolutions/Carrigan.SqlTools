//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.SqlServer.IntegrationTests.DataSets;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Models;

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