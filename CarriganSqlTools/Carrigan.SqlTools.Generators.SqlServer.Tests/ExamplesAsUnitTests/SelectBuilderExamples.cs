using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

public class SelectBuilderExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectWithJoin()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join = new(predicate);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithJoinsAndOrderBy()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderByItem<Order> validates the names of the properties, and throws an error if the property isn't valid
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
    public void SelectWithJoinsWhereAndOrderBy()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: Columns<Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderBy<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join = new(predicate);

        Column<Order> totalCol = new(nameof(Order.Total));
        Parameter minTotal = new(500m, "Total");
        GreaterThan greaterThan = new(totalCol, minTotal);

        OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join,
            Where = greaterThan,
            OrderBys = orderByOrderDate
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) WHERE ([Order].[Total] > @Total_1) ORDER BY [Order].[OrderDate] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 500m);
    }

    [Fact]
    public void SelectWithOffsetNext()
    {
        OffsetFetchNext offsetNext = new(50, 25);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Paging = offsetNext
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Id] ASC OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
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
}
