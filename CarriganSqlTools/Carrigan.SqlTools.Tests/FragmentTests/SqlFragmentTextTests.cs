using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;

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
    public void GetParameters_WithTextFragmentsOnly_ReturnsEmptyDictionary()
    {
        IEnumerable<SqlFragment> fragments =
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentText("1")
        ];

        Dictionary<ParameterTag, object> parameters = fragments.GetParameters();

        Assert.Empty(parameters);
    }

    [Fact]
    public void SqlFragmentText_GetParameters_ReturnsEmptyCollection()
    {
        SqlFragmentText fragment = new("SELECT 1");

        Assert.Empty(fragment.GetParameters());
    }
}
