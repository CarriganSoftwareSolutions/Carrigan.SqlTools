using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class IndexOfWithStringTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new IndexOf(null!, "p")),
        (() => new IndexOf(Value, (string)null!)),
    ];


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new IndexOf(Value, "p"),
        new IndexOf(DifferentParameter, "p"),
        new IndexOf(ColumnA, "p"),
        new IndexOf(ColumnB, "p"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new IndexOf(ColumnA, "p"),
        new IndexOf(ColumnA, "other"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new IndexOf(AggregateWithParameterNoColumn, "p"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new IndexOf(Value, "p"),
        new IndexOf(ColumnA, "p"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new IndexOf(ColumnA, "p"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new IndexOf(Value, "p"),
        new IndexOf(AggregateWithParameterNoColumn, "p"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new IndexOf(Value, "p"),
        new IndexOf(ColumnA, "p"),
        new IndexOf(AggregateWithParameterNoColumn, "p"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
        [];

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
