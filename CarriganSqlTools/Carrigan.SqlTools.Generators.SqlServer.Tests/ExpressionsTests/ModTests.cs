using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class ModTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Mod(null!)),
    ];


    private readonly SqlGenerator<Grades> gradesGenerator = new();


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new Mod(AggregateWithParameterNoColumn, ColumnA)),
        (() => new Mod(ColumnA, AggregateWithParameterNoColumn)),
        (() => new Mod(AggregateWithParameterNoColumn, Second, ColumnA)),
        (() => new Mod(ColumnA, Second, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Mod(First, Second),
        new Mod(First, Third),
        new Mod(Second, Third),
        new Mod(ColumnA, ParameterValue),
        new Mod(ColumnB, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Mod(ColumnA, ParameterValue),
        new Mod(ColumnA, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new Mod(AggregateWithParameterNoColumn, Second),
        new Mod(First, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Mod(First, Second),
        new Mod(ColumnA, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Mod(ColumnA, Second),
        new Mod(First, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Mod(First, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Mod(First, Second),
        new Mod(ColumnA, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Mod(ColumnA, ColumnB),
    ];

    [Fact]
    public void TestNumericMod()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Mod
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
        string expectedText = "SELECT ([Grades].[CreditHours] % @Parameter_1) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedText, actualText);

        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "@Parameter_1", 1);
    }

    [Fact]
    public void TestNumericModMultiple()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Mod
                    (
                        new Column<Grades>(nameof(Grades.CreditHours)),
                        new Parameter(1),
                        new  Parameter(2)
                    ),
                    "ArthemicResult"
                )
            )
        };

        SqlQuery sqlQuery = gradesGenerator.Select(selectBuilder);
        string actualText = sqlQuery.QueryText;
        string expectedText = "SELECT ([Grades].[CreditHours] % @Parameter_1 % @Parameter_2) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedText, actualText);

        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "@Parameter_1", 1);
        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "@Parameter_2", 2);
    }
}
