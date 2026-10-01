using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class LRTrimTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new LRTrim(null!)),
    ];



    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new LRTrim(ParameterValue),
        new LRTrim(DifferentParameter),
        new LRTrim(ColumnA),
        new LRTrim(ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new LRTrim(new Add(ColumnA, ParameterValue)),
        new LRTrim(new Add(ColumnA, ParameterDifferentValue)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new LRTrim(AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new LRTrim(ParameterValue),
        new LRTrim(ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new LRTrim(ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new LRTrim(ParameterValue),
        new LRTrim(AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new LRTrim(ParameterValue),
        new LRTrim(AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new LRTrim(ColumnA),
    ];

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