using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class NegateTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Negate(null!)),
    ];

    private readonly SqlGenerator<Grades> gradesGenerator = new();

    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Negate(ParameterValue),
        new Negate(DifferentParameter),
        new Negate(ColumnA),
        new Negate(ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Negate(new Add(ColumnA, ParameterValue)),
        new Negate(new Add(ColumnA, ParameterDifferentValue)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new Negate(AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Negate(ParameterValue),
        new Negate(ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Negate(ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Negate(ParameterValue),
        new Negate(AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Negate(ParameterValue),
        new Negate(AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Negate(ColumnA),
    ];

    [Fact]
    public void TestNumericNegate()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Negate
                    (
                        new Column<Grades>(nameof(Grades.CreditHours))
                    ),
                    "ArthemicResult"
                )
            )
        };
        SqlQuery sqlQuery = gradesGenerator.Select(selectBuilder);
        string actualText = sqlQuery.QueryText;
        string expectedText = "SELECT (-\"Grades\".\"CreditHours\") AS \"ArthemicResult\" FROM \"Grades\"";
        Assert.Equal(expectedText, actualText);
    }
    [Fact]
    public void TestNumericNegate_LooseColumn()
    {
        Negate negate = new(new Column<Grades>(nameof(Grades.CreditHours)));
        string actualText = negate.ToSqlFragments(new PostgreSqlDialect()).ToSql(new PostgreSqlDialect());
        string expectedText = "(-\"Grades\".\"CreditHours\")";

        Assert.Equal(expectedText, actualText);
    }

}
