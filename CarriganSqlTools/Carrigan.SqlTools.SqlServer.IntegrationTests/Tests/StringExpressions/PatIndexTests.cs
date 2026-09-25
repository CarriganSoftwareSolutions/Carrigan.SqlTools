using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.StringExpressions;

public sealed class PatIndexTests : IClassFixture<LeftFixture>
{
    private readonly LeftFixture _fixture;
    private readonly SqlGenerator<LeftWords> _generator = new();

    public PatIndexTests(LeftFixture fixture) =>
        _fixture = fixture;

    private async Task<IEnumerable<IntegerValue>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<LeftWords> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag<LeftWords>(nameof(LeftWords.Id)),
                new SelectTag(expression, nameof(IntegerValue.Value))
            )
        };

        SqlQuery query = _generator.Select(selectBuilder);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<IntegerValue>(query, null, connection);
    }

    private static void AssertValues(IEnumerable<IntegerValue> records, Dictionary<int, int> expected)
    {
        IntegerValue[] actual = [.. records];
        Assert.Equal(expected.Count, actual.Length);

        foreach (IntegerValue record in actual)
        {
            Assert.True(expected.TryGetValue(record.Id, out int value));
            Assert.Equal(value, record.Value);
        }
    }

    [Fact]
    public async Task StringPattern_ReturnsFirstWildcardMatch()
    {
        IEnumerable<IntegerValue> records = await ExecuteAsync
        (
            new PatIndex("%[aeiou]%", new Column<LeftWords>(nameof(LeftWords.LeftWord)))
        );

        AssertValues(records, new Dictionary<int, int>
        {
            { 1, 1 },
            { 2, 2 },
            { 3, 3 },
            { 4, 2 },
            { 5, 2 }
        });
    }

    [Fact]
    public async Task ExpressionPattern_ReturnsOneBasedPosition()
    {
        IEnumerable<IntegerValue> records = await ExecuteAsync
        (
            new PatIndex(new Parameter("%[0-9]%"), new Parameter("ABC123"))
        );

        Assert.All(records, record => Assert.Equal(4, record.Value));
    }
}
