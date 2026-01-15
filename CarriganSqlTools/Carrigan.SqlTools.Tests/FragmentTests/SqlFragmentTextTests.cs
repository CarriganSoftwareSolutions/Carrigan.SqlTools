using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Tests.FragmentTests;

public class SqlFragmentTextTests
{
    [Fact]
    public void Constructor_WhenTextIsNull_ThrowsArgumentNullException() => 
        _ = Assert.Throws<ArgumentNullException>(() => new SqlFragmentText(null!));

    [Fact]
    public void ToSql_ReturnsProvidedText()
    {
        SqlFragmentText fragment = new("  SELECT 1  ");

        string sql = fragment.ToSql();

        Assert.Equal("  SELECT 1  ", sql);
    }
}
