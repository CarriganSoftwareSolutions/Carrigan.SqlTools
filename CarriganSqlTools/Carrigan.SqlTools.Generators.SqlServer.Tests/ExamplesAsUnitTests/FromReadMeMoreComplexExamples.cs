using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer, Order, PhoneModel, EmailModel and ProcedureExec defined.
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

public class FromReadMeMoreComplexExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();

    [Fact]
    public void SelectWithJoinsAndOrderBy()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderBy<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join = new(predicate);

        OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join,
            OrderBys = orderByOrderDate
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) ORDER BY [Order].[OrderDate] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithTwoPartOrderBy()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderBy<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));

        InnerJoin<Order> join = new(predicate);

        OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));
        OrderBy<Customer> orderByCustomerId = new(nameof(Customer.Id), SortDirectionEnum.Descending);
        OrderBys orderBys = new(orderByCustomerId, orderByOrderDate);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join,
            OrderBys = orderBys
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) ORDER BY [Customer].[Id] DESC, [Order].[OrderDate] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void DeleteWithJoinAndWhere()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValue<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));

        InnerJoin<Customer> join = new(predicate);

        ColumnValue<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");

        DeleteBuilder<Order> deleteBuilder = new()
        {
            Joins = join,
            Where = customerEmail
        };

        SqlQuery query = orderGenerator.Delete(deleteBuilder);

        Assert.Equal("DELETE [Order] FROM [Order] INNER JOIN [Customer] ON ([Customer].[Id] = [Order].[CustomerId]) WHERE ([Customer].[Email] = @Email_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_1", "spam@example.com");
    }

    [Fact]
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SelectCountWithWhere()
    {
        //Note: Column<T> validates the names of the properties, and throws an error if the property isn't valid
        Column<Order> totalCol = new(nameof(Order.Total));
        Parameter minTotal = new(500m, "Total");
        GreaterThan greaterThan = new(totalCol, minTotal);

        SqlQuery query = orderGenerator.SelectCount(null, null, null, greaterThan);

        Assert.Equal("SELECT COUNT([Order].[Id]) FROM [Order] WHERE ([Order].[Total] > @Total_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 500m);
    }

    [Fact]
    public void UpdateWithJoinsAndWhere()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValue<T> validates the names of the properties, and throws an error if the property isn't valid

        Order entity = new() { Id = 10, Total = 123.45m };

        ColumnCollection<Order> columnCollection = new(nameof(Order.Total));

        ColumnEqualsColumn<Order, Customer> predicate = new(nameof(Order.CustomerId), nameof(Customer.Id));

        InnerJoin<Customer> join = new(predicate);

        ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");

        UpdateBuilder<Order> updateBuilder = new()
        {
            Values = entity,
            UpdateColumns = columnCollection,
            Joins = join,
            Where = customerEmailEquals
        };

        SqlQuery query = orderGenerator.Update(updateBuilder);


        Assert.Equal("UPDATE [Order] SET [Order].[Total] = @Total_1 FROM [Order] INNER JOIN [Customer] ON ([Order].[CustomerId] = [Customer].[Id]) WHERE ([Customer].[Email] = @Email_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 123.45m);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "spam@example.com");
    }

    [Fact]
    public void SelectWithAggregatesAndGroupBys()
    {
        Column<Grades> gradePoint = new(nameof(Grades.GradePoint));

        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                SelectTagGenerator.Get<Grades>(nameof(Grades.StudentId)),
                SelectTagGenerator.Get<Grades>(nameof(Grades.CourseCode)),
                new SelectTag(new Average(gradePoint), "AverageGradePoint"),
                new SelectTag(new Sum(gradePoint), "TotalGradePoints"),
                new SelectTag(new Min(gradePoint), "MinimumGradePoint"),
                new SelectTag(new Max(gradePoint), "MaximumGradePoint"),
                new SelectTag(new Count(gradePoint), "GradePointCount")
            ),
            GroupBys = GroupBys
                .New<Grades>(nameof(Grades.StudentId))
                .Append<Grades>(nameof(Grades.CourseCode))
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT [Grades].[StudentId], [Grades].[CourseCode], AVG([Grades].[GradePoint]) AS [AverageGradePoint], SUM([Grades].[GradePoint]) AS [TotalGradePoints], MIN([Grades].[GradePoint]) AS [MinimumGradePoint], MAX([Grades].[GradePoint]) AS [MaximumGradePoint], COUNT([Grades].[GradePoint]) AS [GradePointCount] FROM [Grades] GROUP BY [Grades].[StudentId], [Grades].[CourseCode]",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithAggregatesGroupBysAndHaving()
    {
        Average semesterGpa = new(new Column<Grades>(nameof(Grades.GradePoint)));

        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                SelectTagGenerator.Get<Grades>(nameof(Grades.StudentId)),
                SelectTagGenerator.Get<Grades>(nameof(Grades.AcademicYear)),
                SelectTagGenerator.Get<Grades>(nameof(Grades.SemesterNumber)),
                new SelectTag(semesterGpa, "SemesterGPA")
            ),
            GroupBys = GroupBys.New<Grades>(nameof(Grades.StudentId), nameof(Grades.AcademicYear), nameof(Grades.SemesterNumber)),
            Having = new GreaterThan(semesterGpa, new Parameter(3.5, "HonorRollGpa"))
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal("SELECT [Grades].[StudentId], [Grades].[AcademicYear], [Grades].[SemesterNumber], AVG([Grades].[GradePoint]) AS [SemesterGPA] FROM [Grades] GROUP BY [Grades].[StudentId], [Grades].[AcademicYear], [Grades].[SemesterNumber] HAVING (AVG([Grades].[GradePoint]) > @HonorRollGpa_1)", query.QueryText);
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
        string expectedSql = "SELECT ([Grades].[CreditHours] + @Parameter_1) AS [ArthemicResult] FROM [Grades]";

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
        string expectedSql = "SELECT ([Grades].[CreditHours] % @Parameter_1) AS [ArthemicResult] FROM [Grades]";

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
                    ),
                    "ArthemicResult"
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT (-[Grades].[CreditHours]) AS [ArthemicResult] FROM [Grades]";

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
                    ),
                    "ArthemicResult"
                )
            )
        };

        SqlQuery sqlQuery = selectBuilder.AsSqlQuery();
        string expectedSql = "SELECT ([Grades].[CreditHours] - @Parameter_1) AS [ArthemicResult] FROM [Grades]";

        Assert.Equal(expectedSql, sqlQuery.QueryText);
    }
}