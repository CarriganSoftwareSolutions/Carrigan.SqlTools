using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class IndexOfTests
{
    private static Parameter Value => new("Apple", "Value");
    private static Parameter Find => new("p", "Find");

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

    [Fact]
    public void IsAggregate_UsesSearchedExpression() =>
        Assert.True(new IndexOf(new Count(Value), Find).IsAggregate());
}
