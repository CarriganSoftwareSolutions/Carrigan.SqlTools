//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

using Carrigan.SqlTools.PostgreSql.IntegrationTests.DataSets;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class DeleteFixture : PostgreSqlFixtureBase
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