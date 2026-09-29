using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class FromReadMeSqlExpressionAggregateExamples
{
    [Fact]
    public void GeneralExample()
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
            GroupBys = new GroupBys<Grades>(nameof(Grades.StudentId))
                .Append<Grades>(nameof(Grades.CourseCode))
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT \"Grades\".\"StudentId\", \"Grades\".\"CourseCode\", AVG(\"Grades\".\"GradePoint\") AS \"AverageGradePoint\", SUM(\"Grades\".\"GradePoint\") AS \"TotalGradePoints\", MIN(\"Grades\".\"GradePoint\") AS \"MinimumGradePoint\", MAX(\"Grades\".\"GradePoint\") AS \"MaximumGradePoint\", COUNT(\"Grades\".\"GradePoint\") AS \"GradePointCount\" FROM \"Grades\" GROUP BY \"Grades\".\"StudentId\", \"Grades\".\"CourseCode\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);

    }

    [Fact]
    public void SelectAverageGradePoint()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Average(new Column<Grades>(nameof(Grades.GradePoint))),
                    "OverallAverageGradePoint"
                )
            )
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT AVG(\"Grades\".\"GradePoint\") AS \"OverallAverageGradePoint\" FROM \"Grades\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectDistinctAverageGradePoint()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Average(new Column<Grades>(nameof(Grades.GradePoint)), true),
                    "OverallAverageGradePoint"
                )
            )
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT AVG(DISTINCT \"Grades\".\"GradePoint\") AS \"OverallAverageGradePoint\" FROM \"Grades\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectAvgGradePoint()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Avg(new Column<Grades>(nameof(Grades.GradePoint))),
                    "OverallAvgGradePoint"
                )
            )
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT AVG(\"Grades\".\"GradePoint\") AS \"OverallAvgGradePoint\" FROM \"Grades\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectDistinctAvgGradePoint()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Avg(new Column<Grades>(nameof(Grades.GradePoint)), true),
                    "OverallAvgGradePoint"
                )
            )
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT AVG(DISTINCT \"Grades\".\"GradePoint\") AS \"OverallAvgGradePoint\" FROM \"Grades\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectGradePointCount()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Count(new Column<Grades>(nameof(Grades.GradePoint))),
                    "GradePointCount"
                )
            )
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT COUNT(\"Grades\".\"GradePoint\") AS \"GradePointCount\" FROM \"Grades\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectAllGradeCount()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Count(),
                    "GradeRecordCount"
                )
            )
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT COUNT(*) AS \"GradeRecordCount\" FROM \"Grades\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectMaximumGradePoint()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Max(new Column<Grades>(nameof(Grades.GradePoint))),
                    "MaximumGradePoint"
                )
            )
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT MAX(\"Grades\".\"GradePoint\") AS \"MaximumGradePoint\" FROM \"Grades\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectDistinctMaximumGradePoint()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Max(new Column<Grades>(nameof(Grades.GradePoint)), true),
                    "MaximumGradePoint"
                )
            )
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT MAX(DISTINCT \"Grades\".\"GradePoint\") AS \"MaximumGradePoint\" FROM \"Grades\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectMinimumGradePoint()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Min(new Column<Grades>(nameof(Grades.GradePoint))),
                    "MinimumGradePoint"
                )
            )
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT MIN(\"Grades\".\"GradePoint\") AS \"MinimumGradePoint\" FROM \"Grades\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectDistinctMinimumGradePoint()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Min(new Column<Grades>(nameof(Grades.GradePoint)), true),
                    "MinimumGradePoint"
                )
            )
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT MIN(DISTINCT \"Grades\".\"GradePoint\") AS \"MinimumGradePoint\" FROM \"Grades\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectTotalGradePoints()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Sum(new Column<Grades>(nameof(Grades.GradePoint))),
                    "TotalGradePoints"
                )
            )
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT SUM(\"Grades\".\"GradePoint\") AS \"TotalGradePoints\" FROM \"Grades\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectDistinctTotalGradePoints()
    {
        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag
                (
                    new Sum(new Column<Grades>(nameof(Grades.GradePoint)), true),
                    "TotalGradePoints"
                )
            )
        };

        SqlQuery query = selectBuilder.AsSqlQuery();

        Assert.Equal
        (
            "SELECT SUM(DISTINCT \"Grades\".\"GradePoint\") AS \"TotalGradePoints\" FROM \"Grades\"",
            query.QueryText
        );
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }
}
