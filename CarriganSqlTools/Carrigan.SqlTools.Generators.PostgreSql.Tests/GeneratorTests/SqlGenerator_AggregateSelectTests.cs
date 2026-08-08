using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.GeneratorTests;

public sealed class SqlGenerator_AggregateSelectTests
{
    private static readonly SqlGenerator<Grades> gradesGenerator = new();
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void Select_WithAggregateOnly_AllowsAggregateSelectList()
    {
        SelectTags selects = new(new SelectTag(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount"));

        SqlQuery query = customerGenerator.InternalSelect(null, null, selects, null, null, null, null, null, null);

        Assert.Equal("SELECT COUNT(\"Customer\".\"Id\") AS \"TotalCount\" FROM \"Customer\"", query.QueryText);
    }

    [Fact]
    public void Select_WithCountStar_AllowsAggregateSelectListWithoutSelectedTableTag()
    {
        SelectTags selects = new(new SelectTag(new Count(), "TotalCount"));

        SqlQuery query = customerGenerator.InternalSelect(null, null, selects, null, null, null, null, null, null);

        Assert.Equal("SELECT COUNT(*) AS \"TotalCount\" FROM \"Customer\"", query.QueryText);
    }

    [Fact]
    public void Select_WithGroupBysAndNoSelects_UsesGroupByColumnsAsSelects()
    {
        GroupBys groupBys = GroupBys.New<Customer>(nameof(Customer.Name));

        SqlQuery query = customerGenerator.InternalSelect(null, null, null, null, null, groupBys, null, null, null);

        Assert.Equal("SELECT \"Customer\".\"Name\" FROM \"Customer\" GROUP BY \"Customer\".\"Name\"", query.QueryText);
    }

    [Fact]
    public void Select_WithGroupedColumnAndAggregate_AllowsAggregateSelectList()
    {
        GroupBys groupBys = GroupBys.New<Customer>(nameof(Customer.Name));
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Name)),
            new SelectTag(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount")
        );

        SqlQuery query = customerGenerator.InternalSelect(null, null, selects, null, null, groupBys, null, null, null);

        Assert.Equal("SELECT \"Customer\".\"Name\", COUNT(\"Customer\".\"Id\") AS \"TotalCount\" FROM \"Customer\" GROUP BY \"Customer\".\"Name\"", query.QueryText);
    }

    [Fact]
    public void Select_WithMixedAggregateAndNonAggregateSelects_Throws()
    {
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Name)),
            new SelectTag(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount")
        );

        Assert.Throws<MixedAggregateSelectException>(() => customerGenerator.InternalSelect(null, null, selects, null, null, null, null, null, null));
    }


    [Fact]
    public void Select_WithGroupedColumnAndAggregateAndHaving_AllowsAggregateSelectList()
    {
        GroupBys groupBys = GroupBys.New<Grades>(nameof(Grades.StudentId), nameof(Grades.AcademicYear), nameof(Grades.SemesterNumber));
        Average semesterGpa = new(new Column<Grades>(nameof(Grades.GradePoint)));

        SelectTags selects = new
        (
            SelectTagGenerator.Get<Grades>(nameof(Grades.StudentId)),
            SelectTagGenerator.Get<Grades>(nameof(Grades.AcademicYear)),
            SelectTagGenerator.Get<Grades>(nameof(Grades.SemesterNumber)),
            new SelectTag(semesterGpa, "SemesterGPA")
        );

        Predicates having = new GreaterThan(semesterGpa, new Parameter(3.5, "HonorRollGpa"));

        SqlQuery query = gradesGenerator.InternalSelect(null, null, selects, null, null, groupBys, having, null, null);
        Assert.Equal("SELECT \"Grades\".\"StudentId\", \"Grades\".\"AcademicYear\", \"Grades\".\"SemesterNumber\", AVG(\"Grades\".\"GradePoint\") AS \"SemesterGPA\" FROM \"Grades\" GROUP BY \"Grades\".\"StudentId\", \"Grades\".\"AcademicYear\", \"Grades\".\"SemesterNumber\" HAVING (AVG(\"Grades\".\"GradePoint\") > $1)", query.QueryText);
    }

    [Fact]
    public void Select_WithHavingFromUnjoinedTable_ThrowsInvalidTableException()
    {
        Predicates having = new GreaterThan
        (
            new Sum(new Column<Order>(nameof(Order.Total))),
            new Parameter(100m, "MinimumTotal")
        );

        Assert.Throws<InvalidTableException>(() =>
            customerGenerator.InternalSelect(null, null, null, null, null, null, having, null, null));
    }

    [Fact]
    public void Select_WithWhereAndHaving_PreservesParameterOrder()
    {
        GroupBys groupBys = GroupBys.New<Grades>(nameof(Grades.StudentId));
        Average averageGradePoint = new(new Column<Grades>(nameof(Grades.GradePoint)));
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Grades>(nameof(Grades.StudentId)),
            new SelectTag(averageGradePoint, "AverageGradePoint")
        );
        Predicates where = new GreaterThan
        (
            new Column<Grades>(nameof(Grades.AcademicYear)),
            new Parameter(2000, "MinimumYear")
        );
        Predicates having = new GreaterThan
        (
            averageGradePoint,
            new Parameter(3.5m, "MinimumGpa")
        );

        SqlQuery query = gradesGenerator.InternalSelect(null, null, selects, null, where, groupBys, having, null, null);

        Assert.Equal("SELECT \"Grades\".\"StudentId\", AVG(\"Grades\".\"GradePoint\") AS \"AverageGradePoint\" FROM \"Grades\" WHERE (\"Grades\".\"AcademicYear\" > $1) GROUP BY \"Grades\".\"StudentId\" HAVING (AVG(\"Grades\".\"GradePoint\") > $2)", query.QueryText);
        Assert.Equal(2, query.Parameters.Count());
    }

    [Fact]
    public void Select_WithAggregateOnlyHavingWithoutGroupBy_RendersExpectedSql()
    {
        Count count = new();
        SelectTags selects = new(new SelectTag(count, "TotalCount"));
        Predicates having = new GreaterThan(count, new Parameter(1, "MinimumCount"));

        SqlQuery query = customerGenerator.InternalSelect(null, null, selects, null, null, null, having, null, null);

        Assert.Equal("SELECT COUNT(*) AS \"TotalCount\" FROM \"Customer\" HAVING (COUNT(*) > $1)", query.QueryText);
    }

}
