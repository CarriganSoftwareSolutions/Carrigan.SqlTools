using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class IndexOfTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new IndexOf(null!, Find)),
        (() => new IndexOf(Value, (SqlExpression)null!)),
        (() => new IndexOf(null!, "p")),
        (() => new IndexOf(Value, (string)null!)),
    ];




    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new IndexOf(AggregateWithParameterNoColumn, ColumnA)),
        (() => new IndexOf(ColumnA, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new IndexOf(First, Second),
        new IndexOf(First, Third),
        new IndexOf(Second, Third),
        new IndexOf(ColumnA, ParameterValue),
        new IndexOf(ColumnB, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new IndexOf(ColumnA, ParameterValue),
        new IndexOf(ColumnA, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new IndexOf(AggregateWithParameterNoColumn, Second),
        new IndexOf(First, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new IndexOf(First, Second),
        new IndexOf(ColumnA, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new IndexOf(ColumnA, Second),
        new IndexOf(First, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new IndexOf(First, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new IndexOf(First, Second),
        new IndexOf(ColumnA, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new IndexOf(ColumnA, ColumnB),
    ];

    [Fact]
    public void Constructor_WithExpression_RendersSqlServerArgumentOrder() =>
        Assert.Equal("CHARINDEX(Find, Value)", new IndexOf(Value, Find).ToString());

    [Fact]
    public void Constructor_WithString_RendersSearchAsParameterFirst() =>
        Assert.Equal("CHARINDEX(Parameter, Value)", new IndexOf(Value, "p").ToString());

    [Fact]
    public void Constructor_WithNullValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new IndexOf(null!, Find));

    [Fact]
    public void Constructor_WithNullFindExpression_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new IndexOf(Value, (SqlExpression)null!));

    [Fact]
    public void Constructor_WithNullFindString_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new IndexOf(Value, (string)null!));

    [Fact]
    public void ChildNodes_PreserveValueFirstApiOrder()
    {
        IndexOf expression = new(Value, Find);
        SqlExpression[] children = [.. expression.ChildNodes];
        
        //For Sql server, it renders in this order, because it use CharIndex as the underlying function.
        Assert.Equal(Find, children[0]);
        Assert.Equal(Value, children[1]);
    }

    [Fact]
    public void StringSearch_IsWrappedInParameter()
    {
        IndexOf expression = new(Value, "p");

        Assert.Equal("p", Assert.IsType<Parameter>(expression.ChildNodes.ElementAt(0)).Value);
    }
    
}
