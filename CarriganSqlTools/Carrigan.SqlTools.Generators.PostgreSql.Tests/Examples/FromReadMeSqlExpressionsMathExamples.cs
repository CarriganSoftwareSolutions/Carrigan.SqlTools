using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class FromReadMeSqlExpressionsMathExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();

    [Fact]
    public void Abs_Example()
    {
        Abs abs = new(new Column<Order>(nameof(Order.Total)));
        SelectTags selects = new(new SelectTag(abs, "AbsValue"));

        SelectBuilder<Order> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = orderGenerator.Select(selectBuilder);

        string expected = "SELECT ABS(\"Order\".\"Total\") AS \"AbsValue\" FROM \"Order\"";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

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
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" + $1) AS \"ArthemicResult\" FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void Ceiling_Example()
    {
        Ceiling expression = new(new Column<Order>(nameof(Order.Total)));

        SelectBuilder<Order> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = orderGenerator.Select(selectBuilder);

        string expected = "SELECT CEILING(\"Order\".\"Total\") AS \"Value\" FROM \"Order\"";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
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
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" / $1) AS \"ArthemicResult\" FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void Floor_Example()
    {
        Floor expression = new(new Column<Order>(nameof(Order.Total)));

        SelectBuilder<Order> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = orderGenerator.Select(selectBuilder);

        string expected = "SELECT FLOOR(\"Order\".\"Total\") AS \"Value\" FROM \"Order\"";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
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
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" - $1) AS \"ArthemicResult\" FROM \"Grades\"";

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
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" % $1) AS \"ArthemicResult\" FROM \"Grades\"";

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
                    ),
                    "ArthemicResult"
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" % $1) AS \"ArthemicResult\" FROM \"Grades\"";

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
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" * $1) AS \"ArthemicResult\" FROM \"Grades\"";

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
        string expectedSql = "SELECT (-\"Grades\".\"CreditHours\") AS \"ArthemicResult\" FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }

    [Fact]
    public void Power_Example()
    {
        Power expression = new(new Column<Order>(nameof(Order.Total)), 2);

        SelectBuilder<Order> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = orderGenerator.Select(selectBuilder);

        string expected = "SELECT POWER(\"Order\".\"Total\", $1) AS \"Value\" FROM \"Order\"";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Round_Example()
    {
        Round expression = new(new Column<Customer>(nameof(Customer.Name)));
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT ROUND(\"Customer\".\"Name\") AS \"Value\" FROM \"Customer\"";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RoundWithPrecision_Example()
    {
        Round expression = new(new Column<Customer>(nameof(Customer.Name)), 2);
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT ROUND(\"Customer\".\"Name\", $1) AS \"Value\" FROM \"Customer\"";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal(2, Assert.Single(query.Parameters).Value);
    }

    [Fact]
    public void Sign_Example()
    {
        Sign expression = new(new Column<Order>(nameof(Order.Total)));
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Order> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = orderGenerator.Select(selectBuilder);

        string expected = "SELECT SIGN(\"Order\".\"Total\") AS \"Value\" FROM \"Order\"";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SquareRoot_Example()
    {
        SquareRoot expression = new(new Column<Order>(nameof(Order.Total)));

        SelectBuilder<Order> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = orderGenerator.Select(selectBuilder);

        string expected = "SELECT SQRT(\"Order\".\"Total\") AS \"Value\" FROM \"Order\"";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
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
                    ),
                    "ArthemicResult"
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (\"Grades\".\"CreditHours\" - $1) AS \"ArthemicResult\" FROM \"Grades\"";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }
}
