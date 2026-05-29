using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void NormalizeTimeZone_DateTimeOffsetNull_ReturnsNull()
    {
        DateTimeOffset? actual = Dialect.NormalizeTimeZone((DateTimeOffset?)null);

        Assert.Null(actual);
    }

    [Fact]
    public void NormalizeTimeZone_DateTimeOffset_ReturnsOriginalValue()
    {
        DateTimeOffset expected = new(2025, 1, 2, 3, 4, 5, TimeSpan.FromHours(-6));

        DateTimeOffset? actual = Dialect.NormalizeTimeZone(expected);

        Assert.Equal(expected, actual);
        DateTimeOffset actualValue = actual.GetValueOrDefault();

        Assert.Equal(expected.Offset, actualValue.Offset);
    }

    [Fact]
    public void NormalizeTimeZone_DateTimeNull_ReturnsNull()
    {
        DateTime? actual = Dialect.NormalizeTimeZone((DateTime?)null);

        Assert.Null(actual);
    }

    [Theory]
    [InlineData(DateTimeKind.Utc)]
    [InlineData(DateTimeKind.Local)]
    [InlineData(DateTimeKind.Unspecified)]
    public void NormalizeTimeZone_DateTime_ReturnsOriginalValue(DateTimeKind kind)
    {
        DateTime expected = new(2025, 1, 2, 3, 4, 5, kind);

        DateTime? actual = Dialect.NormalizeTimeZone(expected);

        Assert.Equal(expected, actual);
        DateTime actualValue = actual.GetValueOrDefault();

        Assert.Equal(kind, actualValue.Kind);
    }
}
