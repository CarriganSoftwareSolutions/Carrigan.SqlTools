using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class CoalesceTests
{
    [Fact]
    public void Constructor_NullValues_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Coalesce((IEnumerable<SqlExpression>)null!));

    [Fact]
    public void Constructor_NoValues_Exception()
    {
        IEnumerable<SqlExpression> values = [];

        Assert.Throws<ArgumentException>(() => new Coalesce(values));
    }

    [Fact]
    public void Constructor_OneValue_Exception()
    {
        IEnumerable<SqlExpression> values = [new Parameter(1, "Only")];

        Assert.Throws<ArgumentException>(() => new Coalesce(values));
    }

    [Fact]
    public void Constructor_NullValue_Exception()
    {
        IEnumerable<SqlExpression> values =
        [
            new Parameter(1, "First"),
            null!
        ];

        Assert.Throws<NullReferenceException>(() => new Coalesce(values));
    }

    [Fact]
    public void ToString_RendersValues() =>
        Assert.Equal
        (
            "COALESCE(First, Second, Third)",
            new Coalesce
            (
                new Parameter(1, "First"),
                new Parameter(2, "Second"),
                new Parameter(3, "Third")
            ).ToString()
        );

    [Fact]
    public void ToString_RendersNestedExpression() =>
        Assert.Equal
        (
            "COALESCE((Left + Right), Fallback)",
            new Coalesce
            (
                new Add(new Parameter(1, "Left"), new Parameter(2, "Right")),
                new Parameter(0, "Fallback")
            ).ToString()
        );

    [Fact]
    public void ChildNodes_ContainsValues()
    {
        Parameter first = new(1, "First");
        Parameter second = new(2, "Second");
        Coalesce coalesce = new(first, second);

        SqlExpression[] expected = [first, second];

        Assert.Equal(expected, coalesce.ChildNodes);
    }

    [Fact]
    public void Equality_UsesValues()
    {
        Coalesce first = new(new Parameter(1, "First"), new Parameter(2, "Second"));
        Coalesce equivalent = new(new Parameter(10, "First"), new Parameter(20, "Second"));
        Coalesce different = new(new Parameter(1, "First"), new Parameter(2, "Different"));

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }

    [Fact]
    public void IsAggregate_NonAggregateValues() =>
        Assert.False(new Coalesce(new Parameter(1, "First"), new Parameter(2, "Second")).IsAggregate());

    [Fact]
    public void IsAggregate_AggregateValues() =>
        Assert.True
        (
            new Coalesce
            (
                new Count(new Parameter(1, "First")),
                new Average(new Parameter(2, "Second"))
            ).IsAggregate()
        );

    [Fact]
    public void IsAggregate_MixedValues_Exception() =>
        Assert.Throws<AggregateInconsistencyException>
        (
            () => new Coalesce
            (
                new Count(new Parameter(1, "Aggregate")),
                new Parameter(2, "NonAggregate")
            ).IsAggregate()
        );
}
