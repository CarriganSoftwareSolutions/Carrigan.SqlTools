using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Tests.FragmentTests;

public class SqlFragmentParameterTests
{
    [Fact]
    public void Constructor_WhenParameterIsNull_ThrowsArgumentNullException() => _
        = Assert.Throws<ArgumentNullException>(() => new SqlFragmentParameter(null!));
}
