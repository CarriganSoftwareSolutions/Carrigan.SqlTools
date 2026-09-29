using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.AggregateLogic;

public sealed class AggregateTests
{
    [Fact]
    public void DistinctAggregates_RenderDistinct()
    {
        Parameter value = new(1, "Value");

        Assert.Equal("AVG(DISTINCT Value)", new Avg(value, true).ToString());
        Assert.Equal("AVG(DISTINCT Value)", new Average(value, true).ToString());
        Assert.Equal("COUNT(DISTINCT Value)", new Count(value, true).ToString());
        Assert.Equal("MAX(DISTINCT Value)", new Max(value, true).ToString());
        Assert.Equal("MIN(DISTINCT Value)", new Min(value, true).ToString());
        Assert.Equal("SUM(DISTINCT Value)", new Sum(value, true).ToString());
    }

    [Fact]
    public void DistinctParticipatesInAggregateEquality()
    {
        Parameter value = new(1, "Value");

        Assert.NotEqual(new Avg(value), new Avg(value, true));
        Assert.NotEqual(new Average(value), new Average(value, true));
        Assert.NotEqual(new Count(value), new Count(value, true));
        Assert.NotEqual(new Max(value), new Max(value, true));
        Assert.NotEqual(new Min(value), new Min(value, true));
        Assert.NotEqual(new Sum(value), new Sum(value, true));
    }

    [Fact]
    public void EquivalentDistinctAggregates_AreEqualAndHaveMatchingHashCodes()
    {
        Parameter first = new(1, "Value");
        Parameter second = new(2, "Value");

        Avg avg = new(first, true);
        Average average = new(second, true);

        Assert.Equal(avg, average);
        Assert.Equal(avg.GetHashCode(), average.GetHashCode());
    }

    [Fact]
    public void OneArgumentConstructors_MatchExplicitNonDistinctOverloads()
    {
        Parameter value = new(1, "Value");

        Assert.Equal(new Avg(value), new Avg(value, false));
        Assert.Equal(new Average(value), new Average(value, false));
        Assert.Equal(new Count(value), new Count(value, false));
        Assert.Equal(new Max(value), new Max(value, false));
        Assert.Equal(new Min(value), new Min(value, false));
        Assert.Equal(new Sum(value), new Sum(value, false));
    }

    [Fact]
    public void DistinctAggregateWithoutInput_Throws()
    {
        Assert.Throws<ArgumentException>(() => new TestAggregate(true));
    }

    private sealed class TestAggregate(bool distinct) : Aggregates("TEST", distinct)
    {
    }
}
