using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

public class ArithmeticExamples
{
    [Fact]
    public void AddColumnAndParameter()
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

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT ([Grades].[CreditHours] + @Parameter_1) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void DivideColumnAndParameter()
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
                        new Parameter(2)
                    ),
                    "ArthemicResult"
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT ([Grades].[CreditHours] / @Parameter_1) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void MinusColumnAndParameter()
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

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT ([Grades].[CreditHours] - @Parameter_1) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void ModColumnAndParameter()
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
                        new Parameter(2)
                    ),
                    "ArthemicResult"
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT ([Grades].[CreditHours] % @Parameter_1) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void MultiplyColumnAndParameter()
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
                        new Parameter(2)
                    ),
                    "ArthemicResult"
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT ([Grades].[CreditHours] * @Parameter_1) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void NegateColumn()
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

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (-[Grades].[CreditHours]) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }
}
