using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class MinusTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Minus(null!, Value)),
        (() => new Minus(Value, null!)),
        (() => new Minus(null!, Value, First)),
        (() => new Minus(Value, null!, First)),
        (() => new Minus(Value, First, null!)),
    ];

    private readonly SqlGenerator<Grades> gradesGenerator = new();


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new Minus(AggregateWithParameterNoColumn, ColumnA)),
        (() => new Minus(ColumnA, AggregateWithParameterNoColumn)),
        (() => new Minus(AggregateWithParameterNoColumn, Second, ColumnA)),
        (() => new Minus(ColumnA, Second, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Minus(First, Second),
        new Minus(First, Third),
        new Minus(Second, Third),
        new Minus(ColumnA, ParameterValue),
        new Minus(ColumnB, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Minus(ColumnA, ParameterValue),
        new Minus(ColumnA, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new Minus(AggregateWithParameterNoColumn, Second),
        new Minus(First, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Minus(First, Second),
        new Minus(ColumnA, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Minus(ColumnA, Second),
        new Minus(First, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Minus(First, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Minus(First, Second),
        new Minus(ColumnA, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Minus(ColumnA, ColumnB),
    ];

    [Fact]
    public void TestNumericMinus()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Minus
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
        string expectedText = "SELECT ([Grades].[CreditHours] - @Parameter_1) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedText, actualText);

        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "@Parameter_1", 1);
    }

    [Fact]
    public void TestNumericMinusMultiple()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Minus
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
        string expectedText = "SELECT ([Grades].[CreditHours] - @Parameter_1 - @Parameter_2) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedText, actualText);

        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "@Parameter_1", 1);
        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "@Parameter_2", 2);
    }
}
