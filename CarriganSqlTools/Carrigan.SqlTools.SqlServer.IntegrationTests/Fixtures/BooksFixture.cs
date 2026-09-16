using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class BooksFixture : SqlFixtureBase
{
    public BooksFixture()
        : base
        (
            [
                Book.CreateTableSqlServer
            ],
            Insert.BookInsertStatement
        )
    {
    }
}
