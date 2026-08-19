using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

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
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" + $1) FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void AddNumericColumnAndParameter()
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
                        new NumericParameter(1)
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" + $1) FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void SubtractColumnAndParameter()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Subtract
                    (
                        new Column<Grades>(nameof(Grades.CreditHours)),
                        new Parameter(1)
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" - $1) FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void SubtractNumericColumnAndParameter()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Subtract
                    (
                        new NumericColumn<Grades>(nameof(Grades.CreditHours)),
                        new NumericParameter(1)
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" - $1) FROM \"Grades\"";

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
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" - $1) FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void MinusNumericColumnAndParameter()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Minus
                    (
                        new NumericColumn<Grades>(nameof(Grades.CreditHours)),
                        new NumericParameter(1)
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" - $1) FROM \"Grades\"";

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
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" * $1) FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void MultiplyNumericColumnAndParameter()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Multiply
                    (
                        new NumericColumn<Grades>(nameof(Grades.CreditHours)),
                        new NumericParameter(2)
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" * $1) FROM \"Grades\"";

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
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" / $1) FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void DivideNumericColumnAndParameter()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Divide
                    (
                        new NumericColumn<Grades>(nameof(Grades.CreditHours)),
                        new NumericParameter(2)
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" / $1) FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void ModuloColumnAndParameter()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Modulo
                    (
                        new Column<Grades>(nameof(Grades.CreditHours)),
                        new Parameter(2)
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" % $1) FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void ModuloNumericColumnAndParameter()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Modulo
                    (
                        new NumericColumn<Grades>(nameof(Grades.CreditHours)),
                        new NumericParameter(2)
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" % $1) FROM \"Grades\"";

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
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" % $1) FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void ModNumericColumnAndParameter()
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
                        new NumericParameter(2)
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" % $1) FROM \"Grades\"";

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
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (-\"Grades\".\"CreditHours\") FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void NegateNumericColumn()
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
                    )
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (-\"Grades\".\"CreditHours\") FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }
}
