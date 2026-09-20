using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class LeftFixture : PostgreSqlFixtureBase
{
    public LeftFixture()
        : base
        (
            [
                Left.CreateTablePostgreSql
            ],

            Insert.LeftInsertStatement
        )
    {
    }
}
