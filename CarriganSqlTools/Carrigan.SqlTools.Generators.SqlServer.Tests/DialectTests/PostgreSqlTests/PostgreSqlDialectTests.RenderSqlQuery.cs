using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void RenderSqlQuery_Fragments_ReturnsSqlQuery()
    {
        ISqlFragment[] fragments = [new SqlFragmentText("SELECT 1")];
        SqlQuery actual = Dialect.RenderSqlQuery(fragments);
        Assert.NotNull(actual);
    }
}
