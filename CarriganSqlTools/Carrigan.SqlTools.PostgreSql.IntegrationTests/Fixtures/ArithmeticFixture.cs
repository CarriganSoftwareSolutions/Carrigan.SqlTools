using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class ArithmeticFixture : PostgreSqlFixtureBase
{
    public ArithmeticFixture()
        : base
        (
            [
                Book.CreateTablePostgreSql
            ],
            Insert.BookInsertStatement
        )
    {
    }
}
