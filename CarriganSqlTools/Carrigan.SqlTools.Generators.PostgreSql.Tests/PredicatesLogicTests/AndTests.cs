using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.PredicateLogicTests;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.PredicatesLogicTests;

public class AndTests : PredicateLogicBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new And(null!, ParameterTrue)),
        (() => new And(ParameterFalse, null!)),
        (() => new And(null!, ParameterTrue, ParameterFalse)),
        (() => new And(ParameterFalse, null!, First)),
        (() => new And(ParameterFalse, ParameterTrue, null!)),
    ];

    private static readonly PostgreSqlDialect Dialect = new();


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        MultipleAttemptMixedAggregateConstructions(values => new And(values));

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
        MultipleExpressionsThatAreNotEqual(values => new And(values));

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
        MultipleExpressionsThatAreEqual(values => new And(values));

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
        MultipleExpressionsThatHaveAggregates(values => new And(values));

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
        MultipleExpressionsThatHaveNoAggregates(values => new And(values));

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
        MultipleExpressionsThatHaveColumns(values => new And(values));

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
        MultipleExpressionsThatHaveNoColumns(values => new And(values));

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
        MultipleExpressionsThatHaveParameters(values => new And(values));

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
        MultipleExpressionsThatHaveNoParameters(values => new And(values));

    [Fact]
    public void And_Empty_ToSql() =>
        Assert.Throws<ArgumentException>(() => new And([]));

    [Fact]
    public void And_Null_ToSql() =>
        Assert.Throws<ArgumentNullException>(() => new And(null!));

    [Fact]
    public void And_Single_ToSql()
    {
        And and = new(
        [
            new Column<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsActive)),
        ]);

        string expected = "\"LogicalPredicateTable\".\"IsActive\"";
        string actual = and.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ToSql()
    {
        And and = CreateAnd();

        string expected = "(\"LogicalPredicateTable\".\"IsActive\" AND ($1 IS NOT NULL) AND ($2 IS NOT NULL) AND \"LogicalPredicateTable\".\"IsEnabled\" AND (\"LogicalPredicateTable\".\"IsVisible\" OR \"LogicalPredicateTable\".\"IsArchived\" OR ($3 IS NOT NULL)))";
        string actual = and.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ParameterCount()
    {
        And and = CreateAnd();

        int actual = and.AllParticipatingParameters.Count();
        int expected = 3;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ColumnsCount()
    {
        And and = CreateAnd();

        int actual = and.AllParticipatingColumns.OfType<Column>().Count();
        int expected = 4;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ParameterValues()
    {
        And and = CreateAnd(3);

        IParameter parameter = and.AllParticipatingParameters.Where(parameter => parameter.Name.ToString() == "P1").Single();
        Assert.NotNull(parameter.Value);
        int actual = (int)parameter.Value;
        int expected = 1;

        Assert.Equal(expected, actual);

        parameter = and.AllParticipatingParameters.Where(parameter => parameter.Name.ToString() == "P2").Single();
        Assert.NotNull(parameter.Value);
        actual = (int)parameter.Value;
        expected = 2;

        Assert.Equal(expected, actual);

        parameter = and.AllParticipatingParameters.Where(parameter => parameter.Name.ToString() == "PA").Single();
        Assert.NotNull(parameter.Value);
        actual = (int)parameter.Value;
        expected = 3;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ColumnName()
    {
        And and = CreateAnd();

        _ = and.AllParticipatingColumns.OfType<Column>().Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == "\"LogicalPredicateTable\".\"IsActive\"").Single();
        _ = and.AllParticipatingColumns.OfType<Column>().Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == "\"LogicalPredicateTable\".\"IsEnabled\"").Single();
        _ = and.AllParticipatingColumns.OfType<Column>().Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == "\"LogicalPredicateTable\".\"IsVisible\"").Single();
        _ = and.AllParticipatingColumns.OfType<Column>().Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == "\"LogicalPredicateTable\".\"IsArchived\"").Single();
        _ = and.AllParticipatingColumns.OfType<Column>().Where(column => column.ColumnInfo.ColumnTag.ToString() == "LogicalPredicateTable.IsArchived").Single();
        _ = and.AllParticipatingColumns.OfType<Column>().Where(column => column.ColumnInfo.ToString() == "LogicalPredicateTable.IsArchived").Single();
    }

    private static And CreateAnd(int nestedParameterValue = 2) =>
        new(
        [
            new Column<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsActive)),
            new IsNotNull(new Parameter(1, "P1")),
            new IsNotNull(new Parameter(2, "P2")),
            new Column<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsEnabled)),
            new Or(
            [
                new Column<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsVisible)),
                new Column<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsArchived)),
                new IsNotNull(new Parameter(nestedParameterValue, "PA")),
            ]),
        ]);
}
