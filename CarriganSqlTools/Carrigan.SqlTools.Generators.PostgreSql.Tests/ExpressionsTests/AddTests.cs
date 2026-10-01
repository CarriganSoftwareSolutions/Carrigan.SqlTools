using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class AddTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Add(null!, Value)),
        (() => new Add(Value, null!)),
        (() => new Add(null!, Value, First)),
        (() => new Add(Value, null!, First)),
        (() => new Add(Value, First, null!)),
    ];


    private readonly SqlGenerator<Grades> gradesGenerator = new();


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new Add(AggregateWithParameterNoColumn, ColumnA)),
        (() => new Add(ColumnA, AggregateWithParameterNoColumn)),
        (() => new Add(AggregateWithParameterNoColumn, Second, ColumnA)),
        (() => new Add(ColumnA, Second, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Add(First, Second),
        new Add(First, Third),
        new Add(Second, Third),
        new Add(ColumnA, ParameterValue),
        new Add(ColumnB, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Add(ColumnA, ParameterValue),
        new Add(ColumnA, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new Add(AggregateWithParameterNoColumn, Second),
        new Add(First, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Add(First, Second),
        new Add(ColumnA, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Add(ColumnA, Second),
        new Add(First, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Add(First, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Add(First, Second),
        new Add(ColumnA, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Add(ColumnA, ColumnB),
    ];

    [Fact]
    public void TestNumericAdd()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Add
                    (
                        new Column<Grades>(nameof(Grades.CreditHours)),
                        new Parameter(1)
                    ),
                    "ArthemicResult"
                )
            )
        };

        SqlQuery sqlQuery = gradesGenerator.Select(selectBuilder);
        string actualText = sqlQuery.QueryText;
        string expectedText = "SELECT (\"Grades\".\"CreditHours\" + $1) AS \"ArthemicResult\" FROM \"Grades\"";

        Assert.Equal(expectedText, actualText);

        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "$1", 1);
    }

    [Fact]
    public void TestNumericAddMultiple()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Add
                    (
                        new Column<Grades>(nameof(Grades.CreditHours)),
                        new Parameter(1),
                        new Parameter(2)
                    ),
                    "ArthemicResult"
                )
            )
        };

        SqlQuery sqlQuery = gradesGenerator.Select(selectBuilder);
        string actualText = sqlQuery.QueryText;
        string expectedText = "SELECT (\"Grades\".\"CreditHours\" + $1 + $2) AS \"ArthemicResult\" FROM \"Grades\"";

        Assert.Equal(expectedText, actualText);

        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "$1", 1);
        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "$2", 2);
    }
}
