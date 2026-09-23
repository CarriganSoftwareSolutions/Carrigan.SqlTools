using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.StringExpressions;

public sealed class RepeatTests : IClassFixture<LeftFixture>
{
    private readonly LeftFixture _fixture;
    private readonly SqlGenerator<LeftWords> _generator = new();

    public RepeatTests(LeftFixture fixture) =>
        _fixture = fixture;

    private async Task<IEnumerable<Words>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<LeftWords> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag<LeftWords>(nameof(LeftWords.Id)),
                new SelectTag(expression, nameof(Words.Word))
            )
        };

        SqlQuery query = _generator.Select(selectBuilder);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<Words>(query, null, connection);
    }

    private static void AssertWords(IEnumerable<Words> records, Dictionary<int, string> expected)
    {
        Words[] actual = [.. records];
        Assert.Equal(expected.Count, actual.Length);

        foreach (Words record in actual)
        {
            Assert.True(expected.TryGetValue(record.Id, out string? value));
            Assert.Equal(value, record.Word);
        }
    }

    [Fact]
    public async Task Repeat_WithConstantCount_RepeatsValue()
    {
        IEnumerable<Words> records = await ExecuteAsync(new Repeat(new Column<LeftWords>(nameof(LeftWords.LeftWord)), 2));

        AssertWords(records, new Dictionary<int, string>
        {
            { 1, "AppleApple" },
            { 2, "RiverRiver" },
            { 3, "CloudCloud" },
            { 4, "GardenGarden" },
            { 5, "ForestForest" }
        });
    }

    [Fact]
    public async Task Repeat_WithExpressionCount_RepeatsValue()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Repeat(new Column<LeftWords>(nameof(LeftWords.LeftWord)), new Parameter(3))
        );

        AssertWords(records, new Dictionary<int, string>
        {
            { 1, "AppleAppleApple" },
            { 2, "RiverRiverRiver" },
            { 3, "CloudCloudCloud" },
            { 4, "GardenGardenGarden" },
            { 5, "ForestForestForest" }
        });
    }
}
