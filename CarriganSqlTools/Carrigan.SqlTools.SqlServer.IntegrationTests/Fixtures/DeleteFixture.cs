//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.DataSets;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Models;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class DeleteFixture : SqlFixtureBase
{
    public DeleteFixture()
        : base
        (
            [
                Address.CreateTableSql,
                Book.CreateTableSql,
                Customer.CreateTableSql,
                BookStats.CreateTableSql,
                Order.CreateTableSql,
                OrderedItem.CreateTableSql,
                Left.CreateTableSql,
                Right.CreateTableSql
            ],

            AddressDataSet.InsertStatement
                .Concat(BookDataSet.InsertStatement)
                .Concat(CustomerDataSet.InsertStatement)
                .Concat(BookStatsDataSet.InsertStatement)
                .Concat(OrderDataSet.InsertStatement)
                .Concat(OrderedItemDataSet.InsertStatement)
                .Concat(LeftDataSet.InsertStatement)
                .Concat(RightDataSet.InsertStatement)
        )
    {
    }
}