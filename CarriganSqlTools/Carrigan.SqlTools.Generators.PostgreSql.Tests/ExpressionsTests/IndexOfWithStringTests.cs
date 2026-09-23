using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class IndexOfWithStringTests
{
    private static Parameter Value => new("Apple", "Value");

    [Fact]
    public void Constructor_NullValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new IndexOf(null!, "p"));

    [Fact]
    public void Constructor_NullFind_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new IndexOf(Value, (string)null!));

    [Fact]
    public void ToString_RendersFindAsParameter() =>
        Assert.Equal("STRPOS(Value, Parameter)", new IndexOf(Value, "p").ToString());

    [Fact]
    public void ChildNodes_ContainsValueAndFindParameter()
    {
        IndexOf expression = new(Value, "p");
        SqlExpression[] children = [.. expression.ChildNodes];

        Assert.Equal(Value, children[0]);
        Assert.Equal("p", Assert.IsType<Parameter>(children[1]).Value);
    }

    [Fact]
    public void IsAggregate_UsesSearchedExpression() =>
        Assert.True(new IndexOf(new Count(Value), "p").IsAggregate());
}
