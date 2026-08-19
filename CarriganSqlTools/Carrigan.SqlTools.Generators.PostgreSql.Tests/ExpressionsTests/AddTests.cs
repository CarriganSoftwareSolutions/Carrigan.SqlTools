using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class AddTests
{
    private readonly SqlGenerator<Grades> gradesGenerator = new();

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
                        new NumericColumn<Grades>(nameof(Grades.CreditHours)),
                        new Parameter(1)
                    )
                )
            )
        };

        SqlQuery sqlQuery = gradesGenerator.Select(selectBuilder);
        string actualText = sqlQuery.QueryText;
        string expectedText = "SELECT (\"Grades\".\"CreditHours\" + $1) FROM \"Grades\"";

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
                        new NumericColumn<Grades>(nameof(Grades.CreditHours)),
                        new Parameter(1),
                        new Parameter(2)
                    )
                )
            )
        };

        SqlQuery sqlQuery = gradesGenerator.Select(selectBuilder);
        string actualText = sqlQuery.QueryText;
        string expectedText = "SELECT (\"Grades\".\"CreditHours\" + $1 + $2) FROM \"Grades\"";

        Assert.Equal(expectedText, actualText);

        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "$1", 1);
        SqlQueryTestHelper.AssertParameterValue(sqlQuery, "$2", 2);
    }
}
