using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class NegateTests
{
    private readonly SqlGenerator<Grades> gradesGenerator = new();
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
                        new NumericColumn<Grades>(nameof(Grades.CreditHours))
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
}
