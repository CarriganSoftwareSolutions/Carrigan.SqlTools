using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class AggregateExamples
{
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
}
