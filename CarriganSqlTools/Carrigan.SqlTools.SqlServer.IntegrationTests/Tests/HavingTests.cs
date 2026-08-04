using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests;

public sealed class HavingTests : IClassFixture<HavingFixture>
{
    private readonly HavingFixture _fixture;

    private readonly SqlGenerator<Grades> GradesSqlGenerator = new();

    public HavingTests(HavingFixture fixture) =>
        _fixture = fixture;

    [Fact]
    public async Task GradesWereWrittenAndReadBack()
    {
        await _fixture.ResetAsync();

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);

        foreach (Grades expected in GradesDataSet.Data)
        {
            SqlQuery query = GradesSqlGenerator.SelectById(expected);

            List<Grades> rows =
            [
                .. await CommandsAsync.ExecuteReaderAsync<Grades>
                (
                    query,
                    transaction: null,
                    connection
                )
            ];

            Assert.Single(rows);
            GradesDataSet.Validate(rows[0], expected);
        }
    }

    [Fact]
    public async Task HavingAverageGreaterThanThreePointFive()
    {
        await _fixture.ResetAsync();

        Average average = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Min min = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Max max = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Count count = new(new Column<Grades>(nameof(Grades.GradePoint)));

        Predicates having = new GreaterThan(average, new Parameter(3.50m, "MinimumAverageGpa"));

        SqlQuery query = BuildHavingQuery(average, min, max, count, having);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);

        IEnumerable<GradeResults> results = await CommandsAsync.ExecuteReaderAsync<GradeResults>(query, null, connection);

        AverageGradesDataSet.Validate(results);
    }

    [Fact]
    public async Task HavingMinLessThanCMinus()
    {
        await _fixture.ResetAsync();

        Average average = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Min min = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Max max = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Count count = new(new Column<Grades>(nameof(Grades.GradePoint)));

        Predicates having = new LessThan(min, new Parameter(1.70m, "CMinusGpa"));

        SqlQuery query = BuildHavingQuery(average, min, max, count, having);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);

        IEnumerable<GradeResults> results =
            await CommandsAsync.ExecuteReaderAsync<GradeResults>(query, null, connection);

        MinGradesDataSet.Validate(results);
    }

    [Fact]
    public async Task HavingMaxGreaterThanThreePointFive()
    {
        await _fixture.ResetAsync();

        Average average = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Min min = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Max max = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Count count = new(new Column<Grades>(nameof(Grades.GradePoint)));

        Predicates having = new GreaterThan (max, new Parameter(3.50m, "MinimumMaximumGpa"));

        SqlQuery query = BuildHavingQuery(average, min, max, count, having);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);

        IEnumerable<GradeResults> results =  await CommandsAsync.ExecuteReaderAsync<GradeResults>(query, null, connection);

        MaxGradesDataSet.Validate(results);
    }

    [Fact]
    public async Task HavingCountGreaterThanOne()
    {
        await _fixture.ResetAsync();

        Average average = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Min min = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Max max = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Count count = new(new Column<Grades>(nameof(Grades.GradePoint)));

        Predicates having = new GreaterThan(count,  new Parameter(1, "MinimumCourseCount"));

        SqlQuery query = BuildHavingQuery(average, min, max, count, having);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);

        IEnumerable<GradeResults> results = await CommandsAsync.ExecuteReaderAsync<GradeResults>(query, null, connection);

        CountGradesDataSet.Validate(results);
    }

    private SqlQuery BuildHavingQuery(Average average,  Min min, Max max, Count count, Predicates having)
    {
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Grades>(nameof(Grades.StudentId)),
            SelectTagGenerator.Get<Grades>(nameof(Grades.AcademicYear)),
            SelectTagGenerator.Get<Grades>(nameof(Grades.SemesterNumber)),
            new SelectTag(average, nameof(GradeResults.AverageGPA)),
            new SelectTag(min, nameof(GradeResults.MinGPA)),
            new SelectTag(max, nameof(GradeResults.MaxGPA)),
            new SelectTag(count, nameof(GradeResults.Count))
        );

        GroupBys groupBys = GroupBys.New<Grades>
        (
            nameof(Grades.StudentId),
            nameof(Grades.AcademicYear),
            nameof(Grades.SemesterNumber)
        );

        SelectBuilder<Grades> selectBuilder = new()
        {
            Selects = selects,
            GroupBys = groupBys,
            Having = having
        };

        return GradesSqlGenerator.Select(selectBuilder);
    }
}
