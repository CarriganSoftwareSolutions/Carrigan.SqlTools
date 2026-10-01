using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsNoArgumentBase : SqlExpressionsBaseTests
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New();

    protected virtual bool RenderParentheses =>
        true;


    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
        [];

    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        New(),
        New(),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        New(),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        New(),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        New(),
    ];

    [Fact]
    public void ToString_RendersValues() =>
        Assert.Equal($"{ExpectedFunctionName}{(RenderParentheses ? "()" : string.Empty)}", New().ToString());

    [Fact]
    public void NewEqualsNew() =>
        Assert.Equal(New(), New());

    [Fact]
    public void HasColumns() =>
        Assert.False(New().HasColumns());

    [Fact]
    public void IsAggregate_AggregateValues() =>
        Assert.False(New().IsAggregate());
}
