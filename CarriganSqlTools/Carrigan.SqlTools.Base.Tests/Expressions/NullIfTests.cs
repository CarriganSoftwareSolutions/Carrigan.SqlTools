using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class NullIfTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new NullIf(null!, null!)),
        (() => new NullIf(First, null!)),
        (() => new NullIf(null!, Second)),
    ];

    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new NullIf(AggregateWithParameterNoColumn, ColumnA)),
        (() => new NullIf(ColumnA, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new NullIf(First, Second),
        new NullIf(First, Third),
        new NullIf(Second, Third),
        new NullIf(ColumnA, ParameterValue),
        new NullIf(ColumnB, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new NullIf(ColumnA, ParameterValue),
        new NullIf(ColumnA, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new NullIf(AggregateWithParameterNoColumn, Second),
        new NullIf(First, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new NullIf(First, Second),
        new NullIf(ColumnA, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new NullIf(ColumnA, Second),
        new NullIf(First, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new NullIf(First, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new NullIf(First, Second),
        new NullIf(ColumnA, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new NullIf(ColumnA, ColumnB),
    ];

    [Fact]
    public void Constructor_NullValueLeft_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new NullIf(null!, new Parameter(1)));
    [Fact]
    public void Constructor_NullValueRight_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new NullIf(new Parameter(1), null!));


    [Fact]
    public void ToString_RendersValues() =>
        Assert.Equal
        (
            "NULLIF(First, Second)",
            new NullIf
            (
                new Parameter(1, "First"),
                new Parameter(2, "Second")
            ).ToString()
        );

    [Fact]
    public void ToString_RendersNestedExpression() =>
        Assert.Equal
        (
            "NULLIF((Left + Right), Fallback)",
            new NullIf
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
        NullIf nullif = new(first, second);

        SqlExpression[] expected = [first, second];

        Assert.Equal(expected, nullif.ChildNodes);
    }

    [Fact]
    public void Equality_UsesValues()
    {
        NullIf first = new(new Parameter(1, "First"), new Parameter(2, "Second"));
        NullIf equivalent = new(new Parameter(10, "First"), new Parameter(20, "Second"));
        NullIf different = new(new Parameter(1, "First"), new Parameter(2, "Different"));

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }
}
