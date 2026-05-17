//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Microsoft.Data.SqlClient;
using Respawn;

namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

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