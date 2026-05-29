namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void NormalizeTimeZone_DateTimeOffsetNull_ReturnsNull()
    {
        DateTimeOffset? actual = Dialect.NormalizeTimeZone((DateTimeOffset?)null);

        Assert.Null(actual);
    }

    [Fact]
    public void NormalizeTimeZone_DateTimeOffset_ReturnsUtcValue()
    {
        DateTimeOffset value = new(2025, 1, 2, 3, 4, 5, TimeSpan.FromHours(-6));
        DateTimeOffset expected = new(2025, 1, 2, 9, 4, 5, TimeSpan.Zero);

        DateTimeOffset? actual = Dialect.NormalizeTimeZone(value);

        Assert.Equal(expected, actual);
        DateTimeOffset actualValue = actual.GetValueOrDefault();

        Assert.Equal(TimeSpan.Zero, actualValue.Offset);
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
    public void NormalizeTimeZone_DateTime_RemovesKindWithoutChangingClockTime(DateTimeKind kind)
    {
        DateTime value = new(2025, 1, 2, 3, 4, 5, kind);
        DateTime expected = new(2025, 1, 2, 3, 4, 5, DateTimeKind.Unspecified);

        DateTime? actual = Dialect.NormalizeTimeZone(value);

        Assert.Equal(expected, actual);
        DateTime actualValue = actual.GetValueOrDefault();

        Assert.Equal(DateTimeKind.Unspecified, actualValue.Kind);
    }
}
