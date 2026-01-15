using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Tests.Fragments;

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
}
