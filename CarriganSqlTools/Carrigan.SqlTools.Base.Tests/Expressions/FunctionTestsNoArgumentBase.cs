using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsNoArgumentBase
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New();

    protected virtual bool RenderParentheses =>
        true;

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
