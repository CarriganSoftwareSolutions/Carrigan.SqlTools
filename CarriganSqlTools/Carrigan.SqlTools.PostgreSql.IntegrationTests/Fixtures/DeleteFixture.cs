//Ignore Spelling: Localdb, Respawn, Respawner, Reseed

using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class DeleteFixture : PostgreSqlFixtureBase
{
    public DeleteFixture()
        : base
        (
            [
                Address.CreateTablePostgreSql,
                Book.CreateTablePostgreSql,
                Customer.CreateTablePostgreSql,
                BookStats.CreateTablePostgreSql,
                Order.CreateTablePostgreSql,
                OrderedItem.CreateTablePostgreSql,
                Left.CreateTablePostgreSql,
                Right.CreateTablePostgreSql
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