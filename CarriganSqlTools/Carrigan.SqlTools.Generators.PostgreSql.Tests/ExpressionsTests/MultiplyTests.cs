using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;


public class MultiplyTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Multiply(null!, Value)),
        (() => new Multiply(Value, null!)),
        (() => new Multiply(null!, Value, First)),
        (() => new Multiply(Value, null!, First)),
        (() => new Multiply(Value, First, null!)),
    ];


    private readonly SqlGenerator<Grades> gradesGenerator = new();



    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new Multiply(AggregateWithParameterNoColumn, ColumnA)),
        (() => new Multiply(ColumnA, AggregateWithParameterNoColumn)),
        (() => new Multiply(AggregateWithParameterNoColumn, Second, ColumnA)),
        (() => new Multiply(ColumnA, Second, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Multiply(First, Second),
        new Multiply(First, Third),
        new Multiply(Second, Third),
        new Multiply(ColumnA, ParameterValue),
        new Multiply(ColumnB, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Multiply(ColumnA, ParameterValue),
        new Multiply(ColumnA, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new Multiply(AggregateWithParameterNoColumn, Second),
        new Multiply(First, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Multiply(First, Second),
        new Multiply(ColumnA, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Multiply(ColumnA, Second),
        new Multiply(First, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Multiply(First, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Multiply(First, Second),
        new Multiply(ColumnA, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Multiply(ColumnA, ColumnB),
    ];

    [Fact]
    public void TestNumericMultiply()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Multiply
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
        string expectedText = "SELECT (\"Grades\".\"CreditHours\" * $1) AS \"ArthemicResult\" FROM \"Grades\"";


        Assert.Equal(expectedText, actualText);


        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "$1", 1);
    }


    [Fact]
    public void TestNumericMultiplyMultiple()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Multiply
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
        string expectedText = "SELECT (\"Grades\".\"CreditHours\" * $1 * $2) AS \"ArthemicResult\" FROM \"Grades\"";


        Assert.Equal(expectedText, actualText);


        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "$1", 1);
        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "$2", 2);
    }
}
