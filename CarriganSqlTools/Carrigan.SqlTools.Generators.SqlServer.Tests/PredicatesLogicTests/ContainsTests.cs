using Carrigan.SqlTools.Base.Tests.PredicateLogicTests;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class ContainsTests : PredicateLogicBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Contains<ColumnTable>(new Column<ColumnTable>(nameof(ColumnTable.Col1)), null!)),
    ];

    private static readonly SqlServerDialect Dialect = new();


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Contains<ColumnTable>(new Column<ColumnTable>(nameof(ColumnTable.Col1)), new Parameter("test", "Col1")),
        new Contains<ColumnTable>(new Column<ColumnTable>(nameof(ColumnTable.Col2)), new Parameter("test", "Col2")),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Contains<ColumnTable>(new Column<ColumnTable>(nameof(ColumnTable.Col1)), new Parameter("test", "Col1")),
        new Contains<ColumnTable>(new Column<ColumnTable>(nameof(ColumnTable.Col1)), new Parameter("different", "Col1")),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Contains<ColumnTable>(new Column<ColumnTable>(nameof(ColumnTable.Col1)), new Parameter("test", "Col1")),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Contains<ColumnTable>(new Column<ColumnTable>(nameof(ColumnTable.Col1)), new Parameter("test", "Col1")),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Contains<ColumnTable>(new Column<ColumnTable>(nameof(ColumnTable.Col1)), new Parameter("test", "Col1")),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
        [];

    [Fact]
    public void ContainsTest()
    {
        Contains<ColumnTable> contains = new(new Column<ColumnTable>(nameof(ColumnTable.Col1)), new Parameter("test", "Col1"));

        string expected = "CONTAINS([ColumnTable].[Col1], @Col1_1)";
        string actual = contains.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Contains_NullColumn_ThrowsNullReferenceException() =>
    Assert.Throws<ArgumentNullException>(() =>
        new Contains<ColumnTable>(null!, new Parameter("test", "Col1")));

    [Fact]
    public void Contains_NullParameter_ThrowsNullReferenceException() =>
        Assert.Throws<ArgumentNullException>(() =>
            new Contains<ColumnTable>(new Column<ColumnTable>(nameof(ColumnTable.Col1)), null!));

}
