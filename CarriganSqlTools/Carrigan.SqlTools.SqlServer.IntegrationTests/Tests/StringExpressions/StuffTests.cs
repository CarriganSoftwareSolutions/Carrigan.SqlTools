using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.StringExpressions;

public sealed class StuffTests : IClassFixture<LeftFixture>
{
    private readonly LeftFixture _fixture;
    private readonly SqlGenerator<LeftWords> _generator = new();

    public StuffTests(LeftFixture fixture) =>
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
    public async Task ConstantConstructor_ReplacesRequestedRange()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Stuff(new Column<LeftWords>(nameof(LeftWords.LeftWord)), 2, 3, "X")
        );

        AssertWords(records, new Dictionary<int, string>
        {
            { 1, "AXe" },
            { 2, "RXr" },
            { 3, "CXd" },
            { 4, "GXen" },
            { 5, "FXst" }
        });
    }

    [Fact]
    public async Task ExpressionConstructor_ReplacesRequestedRange()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Stuff
            (
                new Column<LeftWords>(nameof(LeftWords.LeftWord)),
                new Parameter(2),
                new Parameter(3),
                new Parameter("X")
            )
        );

        AssertWords(records, new Dictionary<int, string>
        {
            { 1, "AXe" },
            { 2, "RXr" },
            { 3, "CXd" },
            { 4, "GXen" },
            { 5, "FXst" }
        });
    }
}
