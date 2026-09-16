using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public sealed class BooksFixture : PostgreSqlFixtureBase
{
    public BooksFixture()
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
