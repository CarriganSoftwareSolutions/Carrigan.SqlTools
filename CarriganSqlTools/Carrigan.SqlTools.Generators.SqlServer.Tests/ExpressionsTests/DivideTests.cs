using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class DivideTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Divide(null!, Value)),
        (() => new Divide(Value, null!)),
        (() => new Divide(null!, Value, First)),
        (() => new Divide(Value, null!, First)),
        (() => new Divide(Value, First, null!)),
    ];


    private readonly SqlGenerator<Grades> gradesGenerator = new();


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new Divide(AggregateWithParameterNoColumn, ColumnA)),
        (() => new Divide(ColumnA, AggregateWithParameterNoColumn)),
        (() => new Divide(AggregateWithParameterNoColumn, Second, ColumnA)),
        (() => new Divide(ColumnA, Second, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Divide(First, Second),
        new Divide(First, Third),
        new Divide(Second, Third),
        new Divide(ColumnA, ParameterValue),
        new Divide(ColumnB, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Divide(ColumnA, ParameterValue),
        new Divide(ColumnA, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new Divide(AggregateWithParameterNoColumn, Second),
        new Divide(First, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Divide(First, Second),
        new Divide(ColumnA, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Divide(ColumnA, Second),
        new Divide(First, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Divide(First, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Divide(First, Second),
        new Divide(ColumnA, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Divide(ColumnA, ColumnB),
    ];

    [Fact]
    public void TestNumericDivide()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Divide
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
        string expectedText = "SELECT ([Grades].[CreditHours] / @Parameter_1) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedText, actualText);

        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "@Parameter_1", 1);
    }

    [Fact]
    public void TestNumericDivideMultiple()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Divide
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
        string expectedText = "SELECT ([Grades].[CreditHours] / @Parameter_1 / @Parameter_2) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedText, actualText);

        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "@Parameter_1", 1);
        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "@Parameter_2", 2);
    }
}
