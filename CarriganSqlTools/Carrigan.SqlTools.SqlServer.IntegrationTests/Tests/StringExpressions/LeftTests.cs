using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;
using LeftExpression = Carrigan.SqlTools.Expressions.Left;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.StringExpressions;

public sealed class LeftTests : IClassFixture<LeftFixture>
{
    private readonly LeftFixture _fixture;
    private readonly SqlGenerator<LeftWords> _generator = new();

    public LeftTests(LeftFixture fixture) =>
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
    public async Task Left_WithConstantLength_ReturnsLeadingCharacters()
    {
        IEnumerable<Words> records = await ExecuteAsync(new LeftExpression(new Column<LeftWords>(nameof(LeftWords.LeftWord)), 3));

        AssertWords(records, new Dictionary<int, string>
        {
            { 1, "App" },
            { 2, "Riv" },
            { 3, "Clo" },
            { 4, "Gar" },
            { 5, "For" }
        });
    }

    [Fact]
    public async Task Left_WithExpressionLength_ReturnsLeadingCharacters()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new LeftExpression(new Column<LeftWords>(nameof(LeftWords.LeftWord)), new Parameter(2))
        );

        AssertWords(records, new Dictionary<int, string>
        {
            { 1, "Ap" },
            { 2, "Ri" },
            { 3, "Cl" },
            { 4, "Ga" },
            { 5, "Fo" }
        });
    }
}
