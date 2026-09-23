using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;
using RightExpression = Carrigan.SqlTools.Expressions.Right;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.StringExpressions;

public sealed class RightTests : IClassFixture<LeftFixture>
{
    private readonly LeftFixture _fixture;
    private readonly SqlGenerator<LeftWords> _generator = new();

    public RightTests(LeftFixture fixture) =>
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

        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
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
    public async Task Right_WithConstantLength_ReturnsTrailingCharacters()
    {
        IEnumerable<Words> records = await ExecuteAsync(new RightExpression(new Column<LeftWords>(nameof(LeftWords.LeftWord)), 2));

        AssertWords(records, new Dictionary<int, string>
        {
            { 1, "le" },
            { 2, "er" },
            { 3, "ud" },
            { 4, "en" },
            { 5, "st" }
        });
    }

    [Fact]
    public async Task Right_WithExpressionLength_ReturnsTrailingCharacters()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new RightExpression(new Column<LeftWords>(nameof(LeftWords.LeftWord)), new Parameter(3))
        );

        AssertWords(records, new Dictionary<int, string>
        {
            { 1, "ple" },
            { 2, "ver" },
            { 3, "oud" },
            { 4, "den" },
            { 5, "est" }
        });
    }
}
