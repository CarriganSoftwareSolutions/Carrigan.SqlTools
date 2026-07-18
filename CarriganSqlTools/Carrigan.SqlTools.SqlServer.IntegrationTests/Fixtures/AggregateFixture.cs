//Ignore Spelling: Localdb, Respawn, Respawner, Reseed

using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class AggregateFixture : SqlFixtureBase
{
    public AggregateFixture()
        : base
        (
            [
                Address.CreateTableSqlServer,
                Book.CreateTableSqlServer,
                Customer.CreateTableSqlServer,
                Order.CreateTableSqlServer,
                OrderedItem.CreateTableSqlServer
            ],

            Insert.AddressInsertStatement
                .Concat(Insert.BookInsertStatement)
                .Concat(Insert.CustomerInsertStatement)
                .Concat(Insert.OrderInsertStatement)
                .Concat(Insert.OrderedItemInsertStatement)
        )
    {
    }
}