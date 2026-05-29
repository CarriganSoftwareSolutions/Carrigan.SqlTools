using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using System.Data;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void RenderSqlQuery_Fragments_ReturnsSqlQuery()
    {
        ISqlFragment[] fragments = [new SqlFragmentText("SELECT 1")];
        SqlQuery actual = new(Dialect, CommandType.Text, fragments);
        Assert.NotNull(actual);
    }
}
