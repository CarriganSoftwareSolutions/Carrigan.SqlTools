using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class CoalesceFixture : PostgreSqlFixtureBase
{
    public CoalesceFixture()
        : base
        (
            [
                Left.CreateTablePostgreSql,
                Right.CreateTablePostgreSql
            ],

            Insert.LeftInsertStatement
                .Concat(Insert.RightInsertStatement)
        )
    {
    }
}
