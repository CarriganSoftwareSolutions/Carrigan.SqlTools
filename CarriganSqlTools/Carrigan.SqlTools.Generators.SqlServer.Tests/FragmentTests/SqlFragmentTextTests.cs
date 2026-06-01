using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.FragmentTests;

public class SqlFragmentTextTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void Constructor_NullText_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new SqlFragmentText(null!));

    [Fact]
    public void ToSql_ReturnsText()
    {
        SqlFragmentText fragment = new("SELECT 1");

        string actualValue = fragment.ToSql(Dialect);

        Assert.Equal("SELECT 1", actualValue);
    }

    [Fact]
    public void GetParameters_WithTextFragmentsOnly_ReturnsEmptyCollection()
    {
        IEnumerable<ISqlFragment> fragments =
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentText("1")
        ];

        IEnumerable<SqlFragmentParameter> parameters = fragments.GetSqlFragmentParameters(Dialect);

        Assert.Empty(parameters);
    }

    [Fact]
    public void SqlFragmentText_GetParameters_ReturnsEmptyCollection()
    {
        SqlFragmentText fragment = new("SELECT 1");

        Assert.Empty(fragment.GetSqlFragmentParameters());
    }
}