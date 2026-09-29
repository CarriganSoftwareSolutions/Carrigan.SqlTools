using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests;

public sealed class DistinctAggregateTests : IClassFixture<HavingFixture>
{
    private readonly HavingFixture _fixture;
    private readonly SqlGenerator<Grades> _gradesSqlGenerator = new();

    public DistinctAggregateTests(HavingFixture fixture) =>
        _fixture = fixture;

    [Fact]
    public async Task DistinctAggregates_ReturnExpectedValues()
    {
        await _fixture.ResetAsync();

        decimal[] distinctGradePoints = [.. GradesDataSet.Data.Select(static grade => grade.GradePoint).Distinct()];
        Column<Grades> gradePoint = new(nameof(Grades.GradePoint));

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);

        decimal expectedAverage = decimal.Round(distinctGradePoints.Average(), 6);

        Assert.Equal(expectedAverage, await ExecuteDecimalAsync(new Average(gradePoint, true), connection), 5);
        Assert.Equal(expectedAverage, await ExecuteDecimalAsync(new Avg(gradePoint, true), connection), 5);
        Assert.Equal(distinctGradePoints.Length, await ExecuteIntAsync(new Count(gradePoint, true), connection));
        Assert.Equal(distinctGradePoints.Max(), await ExecuteDecimalAsync(new Max(gradePoint, true), connection));
        Assert.Equal(distinctGradePoints.Min(), await ExecuteDecimalAsync(new Min(gradePoint, true), connection));
        Assert.Equal(distinctGradePoints.Sum(), await ExecuteDecimalAsync(new Sum(gradePoint, true), connection));
    }

    private async Task<decimal> ExecuteDecimalAsync(SqlExpression aggregate, SqlConnection connection) =>
        Convert.ToDecimal(await CommandsAsync.ExecuteScalarAsync(BuildQuery(aggregate), null, connection));

    private async Task<int> ExecuteIntAsync(SqlExpression aggregate, SqlConnection connection) =>
        Convert.ToInt32(await CommandsAsync.ExecuteScalarAsync(BuildQuery(aggregate), null, connection));

    private SqlQuery BuildQuery(SqlExpression aggregate) =>
        _gradesSqlGenerator.Select
        (
            new SelectBuilder<Grades>
            {
                Selects = new SelectTags(new SelectTag(aggregate, "Value"))
            }
        );
}
