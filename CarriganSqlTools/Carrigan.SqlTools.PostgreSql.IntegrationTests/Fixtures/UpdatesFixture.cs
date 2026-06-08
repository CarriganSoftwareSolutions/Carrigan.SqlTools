//Ignore Spelling: Localdb, Respawn, Respawner, Reseed

using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class UpdatesFixture : PostgreSqlFixtureBase
{
    public UpdatesFixture()
        : base
        (
            [
                Address.CreateTablePostgreSql,
                Customer.CreateTablePostgreSql,
                Order.CreateTablePostgreSql,
            ],

            Insert.AddressInsertStatement
                .Concat(Insert.CustomerInsertStatement)
                .Concat(Insert.OrderInsertStatement)
        )
    {
    }
}