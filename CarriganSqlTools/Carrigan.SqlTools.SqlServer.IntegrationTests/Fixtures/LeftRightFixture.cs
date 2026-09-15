using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Inserts;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public sealed class LeftRightFixture : SqlFixtureBase
{
    public LeftRightFixture()
        : base
        (
            [
                Left.CreateTableSqlServer,
                Right.CreateTableSqlServer
            ],

            Insert.LeftInsertStatement
                .Concat(Insert.RightInsertStatement)
        )
    {
    }
}
