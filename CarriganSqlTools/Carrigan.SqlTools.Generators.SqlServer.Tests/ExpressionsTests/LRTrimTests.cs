using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class LRTrimTests
{
    [Fact]
    public void Constructor_NullValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new LRTrim(null!));

    [Fact]
    public void ToString_RendersNestedLTrimRTrim() =>
        Assert.Equal
        (
            "LTRIM(RTRIM(Title))",
            new LRTrim(new Parameter("Pride and Prejudice", "Title")).ToString()
        );

    [Fact]
    public void IsAggregate_NonAggregateValue() =>
        Assert.False(new LRTrim(new Parameter("Title")).IsAggregate());

    [Fact]
    public void IsAggregate_AggregateValue() =>
        Assert.True(new LRTrim(new Count(new Parameter("Title"))).IsAggregate());
}