using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Tests.FragmentTests;

public class SqlFragmentExtensionsTests
{
    [Fact]
    public void ToSql_Empty_ReturnsEmptyString()
    {
        IEnumerable<SqlFragment> sqlFragments = [];

        string actualValue = sqlFragments.ToSql();

        Assert.Equal(string.Empty, actualValue);
    }

    [Fact]
    public void ToSql_MultipleFragments_ConcatenatesInOrder()
    {
        IEnumerable<SqlFragment> sqlFragments =
            [
                new SqlFragmentText("SELECT "),
                new SqlFragmentText("1"),
                new SqlFragmentText(";")
            ];

        string actualValue = sqlFragments.ToSql();

        Assert.Equal("SELECT 1;", actualValue);
    }

    [Fact]
    public void ToSql_NullSource_Exception()
    {
        IEnumerable<SqlFragment> sqlFragments = null!;

        Assert.Throws<ArgumentNullException>(() => sqlFragments.ToSql());
    }

    [Fact]
    public void GetParameters_NullSource_Exception()
    {
        IEnumerable<SqlFragment> sqlFragments = null!;

        Assert.Throws<ArgumentNullException>(() => sqlFragments.GetParameters());
    }

    [Fact]
    public void GetParameters_DuplicateKeys_Exception()
    {
        IEnumerable<SqlFragment> first = new Parameter("Name", 1).ToSqlFragments("Parameter");
        IEnumerable<SqlFragment> second = new Parameter("Name", 2).ToSqlFragments("Parameter");

        IEnumerable<SqlFragment> sqlFragments = [.. first, .. second];

        Assert.Throws<ArgumentException>(() => sqlFragments.GetParameters());
    }
}
