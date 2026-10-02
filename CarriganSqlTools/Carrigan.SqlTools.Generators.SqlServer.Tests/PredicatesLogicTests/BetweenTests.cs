using Carrigan.SqlTools.Base.Tests.PredicateLogicTests;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class BetweenTests : PredicateLogicBaseTests
{
    private static readonly SqlServerDialect Dialect = new();

    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Between(null!, Second, Third)),
        (() => new Between(First, null!, Third)),
        (() => new Between(First, Second, null!)),
    ];

    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        TernaryAttemptMixedAggregateConstructions((value, left, right) => new Between(value, left, right));

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
        TernaryExpressionsThatAreNotEqual((value, left, right) => new Between(value, left, right));

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
        TernaryExpressionsThatAreEqual((value, left, right) => new Between(value, left, right));

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
        TernaryExpressionsThatHaveAggregates((value, left, right) => new Between(value, left, right));

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
        TernaryExpressionsThatHaveNoAggregates((value, left, right) => new Between(value, left, right));

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
        TernaryExpressionsThatHaveColumns((value, left, right) => new Between(value, left, right));

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
        TernaryExpressionsThatHaveNoColumns((value, left, right) => new Between(value, left, right));

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
        TernaryExpressionsThatHaveParameters((value, left, right) => new Between(value, left, right));

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
        TernaryExpressionsThatHaveNoParameters((value, left, right) => new Between(value, left, right));

    [Fact]
    public void ToSql_ColumnWithParameterBounds()
    {
        Column<ColumnTable> value = new(nameof(ColumnTable.Pizza));
        Parameter minimum = new(10, "Minimum");
        Parameter maximum = new(20, "Maximum");
        Between predicate = new(value, minimum, maximum);

        string actual = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal("([ColumnTable].[Pizza] BETWEEN @Minimum_1 AND @Maximum_2)", actual);
    }

    [Fact]
    public void AllParticipatingParameters_PreservesBoundOrderAndValues()
    {
        Between predicate = new
        (
            new Column<ColumnTable>(nameof(ColumnTable.Pizza)),
            new Parameter(10, "Minimum"),
            new Parameter(20, "Maximum")
        );

        IParameter[] parameters = [.. predicate.AllParticipatingParameters];

        Assert.Equal(2, parameters.Length);
        Assert.Equal("Minimum", parameters[0].Name.ToString());
        Assert.Equal(10, Assert.IsType<int>(parameters[0].Value));
        Assert.Equal("Maximum", parameters[1].Name.ToString());
        Assert.Equal(20, Assert.IsType<int>(parameters[1].Value));
    }

    [Fact]
    public void Equality_DistinguishesBetweenFromNotBetween()
    {
        SqlExpression between = new Between(First, Second, Third);
        SqlExpression notBetween = new NotBetween(First, Second, Third);

        Assert.NotEqual(between, notBetween);
        Assert.True(between != notBetween);
    }

    [Fact]
    public void AllParticipatingColumns_IncludesValueAndColumnBounds()
    {
        Between predicate = new
        (
            new Column<ColumnTable>(nameof(ColumnTable.Pizza)),
            new Column<ColumnTable>(nameof(ColumnTable.ColA)),
            new Column<ColumnTable>(nameof(ColumnTable.ColB))
        );

        Assert.Equal(3, predicate.AllParticipatingColumns.Count());
        Assert.Single(predicate.AllParticipatingTables);
    }
}
