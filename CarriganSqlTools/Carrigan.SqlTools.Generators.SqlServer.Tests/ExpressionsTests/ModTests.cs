using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class ModTests
{
    private readonly SqlGenerator<Grades> gradesGenerator = new();

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
                        new NumericColumn<Grades>(nameof(Grades.CreditHours)),
                        new NumericParameter<int>(1),
                        new NumericParameter<int>(2)
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
