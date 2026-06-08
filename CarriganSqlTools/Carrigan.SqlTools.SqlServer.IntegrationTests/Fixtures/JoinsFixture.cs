//Ignore Spelling: Localdb, Respawn, Respawner, Reseed

using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class JoinsFixture : SqlFixtureBase
{
    public JoinsFixture()
        : base
        (
            [
                Address.CreateTableSqlServer,
                Book.CreateTableSqlServer,
                Customer.CreateTableSqlServer,
                BookStats.CreateTableSqlServer,
                Order.CreateTableSqlServer,
                OrderedItem.CreateTableSqlServer,
                Left.CreateTableSqlServer,
                Right.CreateTableSqlServer
            ],

            Insert.AddressInsertStatement
                .Concat(Insert.BookInsertStatement)
                .Concat(Insert.CustomerInsertStatement)
                .Concat(Insert.BookStatsInsertStatement)
                .Concat(Insert.OrderInsertStatement)
                .Concat(Insert.OrderedItemInsertStatement)
                .Concat(Insert.LeftInsertStatement)
                .Concat(Insert.RightInsertStatement)
        )
    {
    }
}