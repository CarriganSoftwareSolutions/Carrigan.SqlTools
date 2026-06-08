//Ignore Spelling: Localdb, Respawn, Respawner, Reseed

using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class SubqueryFixture : SqlFixtureBase
{
    public SubqueryFixture()
        : base
        (
            [
                Address.CreateTableSqlServer,
                Customer.CreateTableSqlServer,
                Order.CreateTableSqlServer,
            ],

            Insert.AddressInsertStatement
                .Concat(Insert.CustomerInsertStatement)
                .Concat(Insert.OrderInsertStatement)
        )
    {
    }
}