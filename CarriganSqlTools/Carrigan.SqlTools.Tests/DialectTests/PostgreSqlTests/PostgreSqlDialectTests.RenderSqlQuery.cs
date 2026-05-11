using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void RenderSqlQuery_Fragments_ReturnsSqlQuery()
    {
        SqlFragment[] fragments = [new SqlFragmentText("SELECT 1")];
        SqlQuery actual = Dialect.RenderSqlQuery(fragments);
        Assert.NotNull(actual);
    }
}
