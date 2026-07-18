//Ignore Spelling: Localdb, Respawn, Respawner, Reseed

using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class AggregateFixture : PostgreSqlFixtureBase
{
    public AggregateFixture()
        : base
        (
            [
                Address.CreateTablePostgreSql,
                Book.CreateTablePostgreSql,
                Customer.CreateTablePostgreSql,
                Order.CreateTablePostgreSql,
                OrderedItem.CreateTablePostgreSql
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