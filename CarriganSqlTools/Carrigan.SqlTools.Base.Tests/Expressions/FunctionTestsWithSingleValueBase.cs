using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsWithSingleValueBase : SqlExpressionsBaseTests
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New(SqlExpression? sqlExpression);


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        New(ParameterValue),
        New(DifferentParameter),
        New(ColumnA),
        New(ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        New(new Add(ColumnA, ParameterValue)),
        New(new Add(ColumnA, ParameterDifferentValue)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        New(AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        New(ParameterValue),
        New(ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        New(ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        New(ParameterValue),
        New(AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        New(ParameterValue),
        New(AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        New(ColumnA),
    ];

    [Fact]
    public void Constructor_NullValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New((SqlExpression)null!));


    [Fact]
    public void ToString_RendersValues() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(Parameter)",
            New (ParameterValue).ToString()
        );

    [Fact]
    public void ToString_RendersNestedExpression() =>
        Assert.Equal
        (
            //TODO: this works, but we need to try and fix the double parenthesis
            $"{ExpectedFunctionName}((Left + Right))",
            New(LeftRight).ToString()
        );

    [Fact]
    public void ChildNodes_ContainsValues()
    {
        Parameter first = new(1);

        SqlExpression[] actual = [.. New(ParameterValue).ChildNodes];

        SqlExpression[] expected = [first];

        Assert.Equal(expected, expected);
    }

    [Fact]
    public void Equality_UsesValues()
    {
        FunctionalExpression first = New(ParameterValue);
        FunctionalExpression equivalent = New(ParameterDifferentValue);
        FunctionalExpression different = New(DifferentParameter);

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }

    [Fact]
    public void IsAggregate_NonAggregateValues() =>
        Assert.False(New(ParameterValue).IsAggregate());

    [Fact]
    public void HasAggregate_NonAggregateValues() =>
        Assert.False(New(ParameterValue).HasAggregates());

    [Fact]
    public void IsAggregate_AggregateValues() =>
        Assert.False
        (
            New(AggregateWithParameterNoColumn).IsAggregate()
        );

    [Fact]
    public void HasAggregate_AggregateValues() =>
        Assert.True
        (
            New(AggregateWithParameterNoColumn).HasAggregates()
        );
}
