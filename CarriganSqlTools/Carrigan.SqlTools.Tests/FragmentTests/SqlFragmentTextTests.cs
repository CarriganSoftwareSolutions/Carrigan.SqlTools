using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Tests.FragmentTests;

public class SqlFragmentTextTests
{
    [Fact]
    public void Constructor_NullText_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new SqlFragmentText(null!));

    [Fact]
    public void ToSql_ReturnsText()
    {
        SqlFragmentText fragment = new("SELECT 1");

        string actualValue = fragment.ToSql();

        Assert.Equal("SELECT 1", actualValue);
    }

    [Fact]
    public void GetParameters_WithTextFragmentsOnly_ReturnsEmptyCollection()
    {
        IEnumerable<SqlFragment> fragments =
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentText("1")
        ];

        IEnumerable<SqlFragmentParameter> parameters = fragments.GetSqlFragmentParameters(new SqlServerDialect());

        Assert.Empty(parameters);
    }

    [Fact]
    public void SqlFragmentText_GetParameters_ReturnsEmptyCollection()
    {
        SqlFragmentText fragment = new("SELECT 1");

        Assert.Empty(fragment.GetSqlFragmentParameters());
    }
}