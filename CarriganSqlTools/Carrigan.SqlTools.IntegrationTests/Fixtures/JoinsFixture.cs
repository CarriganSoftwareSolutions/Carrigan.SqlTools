//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Microsoft.Data.SqlClient;
using Respawn;

namespace Carrigan.SqlTools.IntegrationTests.Fixtures;

public sealed class JoinsFixture : SqlFixtureBase
{
    public JoinsFixture()
        : base
        (
            [
                Address.CreateTableSql,
                Book.CreateTableSql,
                Customer.CreateTableSql,
                BookStats.CreateTableSql,
                Order.CreateTableSql,
                OrderedItem.CreateTableSql
            ],

            AddressDataSet.InsertStatement
                .Concat(BookDataSet.InsertStatement)
                .Concat(CustomerDataSet.InsertStatement)
                .Concat(BookStatsDataSet.InsertStatement)
                .Concat(OrderDataSet.InsertStatement)
                .Concat(OrderedItemDataSet.InsertStatement)
        )
    {
    }
}