using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.StringExpressions;

public sealed class LengthTests : IClassFixture<LeftFixture>
{
    private readonly LeftFixture _fixture;
    private readonly SqlGenerator<LeftWords> _generator = new();

    public LengthTests(LeftFixture fixture) =>
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

        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
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
    public async Task Length_ReturnsLength()
    {
        IEnumerable<IntegerValue> records = await ExecuteAsync(new Length(new Column<LeftWords>(nameof(LeftWords.LeftWord))));

        AssertValues(records, new Dictionary<int, int>
        {
            { 1, 5 },
            { 2, 5 },
            { 3, 5 },
            { 4, 6 },
            { 5, 6 }
        });
    }
}
