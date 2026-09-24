using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

//IGNORE SPELLING: Substring, ppl, ive, ard lou

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.StringExpressions;

public sealed class SubstringTests : IClassFixture<LeftFixture>
{
    private readonly LeftFixture _fixture;
    private readonly SqlGenerator<LeftWords> _generator = new();

    public SubstringTests(LeftFixture fixture) =>
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
    public async Task Substring_WithConstants_ReturnsRequestedCharacters()
    {
        IEnumerable<Words> records = await ExecuteAsync(new Substring(new Column<LeftWords>(nameof(LeftWords.LeftWord)), 2, 3));

        AssertWords(records, new Dictionary<int, string>
        {
            { 1, "ppl" },
            { 2, "ive" },
            { 3, "lou" },
            { 4, "ard" },
            { 5, "ore" }
        });
    }

    [Fact]
    public async Task Substring_WithExpressions_ReturnsRequestedCharacters()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Substring(new Column<LeftWords>(nameof(LeftWords.LeftWord)), new Parameter(2), new Parameter(2))
        );

        AssertWords(records, new Dictionary<int, string>
        {
            { 1, "pp" },
            { 2, "iv" },
            { 3, "lo" },
            { 4, "ar" },
            { 5, "or" }
        });
    }
}
