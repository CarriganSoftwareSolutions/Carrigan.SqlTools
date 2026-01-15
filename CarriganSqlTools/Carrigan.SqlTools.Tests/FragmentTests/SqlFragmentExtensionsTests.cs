using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Tests.FragmentTests;

public class SqlFragmentExtensionsTests
{
    [Fact]
    public void ToSql_ConcatenatesFragmentSqlInOrder()
    {
        IEnumerable<SqlFragment> sqlFragments =
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentText("1"),
        ];

        string sql = sqlFragments.ToSql();

        Assert.Equal("SELECT 1", sql);
    }
}
