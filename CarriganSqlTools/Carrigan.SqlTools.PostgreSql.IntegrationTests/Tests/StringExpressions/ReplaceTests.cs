using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.StringExpressions;

public sealed class ReplaceTests : IClassFixture<LeftFixture>
{
    private static readonly ISqlDialects Dialect = new PostgreSqlDialect();
    private readonly LeftFixture _fixture;
    private readonly SqlGenerator<LeftWords> LeftSqlGenerator = new();

    public ReplaceTests(LeftFixture fixture) =>
        _fixture = fixture;

    private async Task<IEnumerable<Words>> ExecuteAsync(SqlExpression expression)
    {
        ColumnEqualsColumn<LeftWords, RightWords> ids = new (nameof(LeftWords.Id), nameof(RightWords.Id));
        SelectBuilder<LeftWords> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new Column<LeftWords>(nameof(LeftWords.Id)).AsSelectTag("Id"),
                new SelectTag(expression, "Word")
            )
        };

        SqlQuery query = LeftSqlGenerator.Select(selectBuilder);

        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<Words>(query, null, connection);
    }

    private static void AssertWords(IEnumerable<Words> records, Dictionary<int, string?> expectedValues)
    {
        Words[] actualRecords = [.. records];

        Assert.Equal(expectedValues.Count, actualRecords.Length);

        foreach (Words actual in actualRecords)
        {
            Assert.True(expectedValues.TryGetValue(actual.Id, out string? expected));

            if (expected is null)
            {
                Assert.Null(actual.Word);
            }
            else
            {
                Assert.NotNull(actual.Word);
                Assert.Equal(expected, actual.Word);
            }
        }
    }

    [Fact]
    public async Task Replace()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Replace(new Column<LeftWords>(nameof(LeftWords.LeftWord)), "r", "l")

        );

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "Apple" },
            { 2, "Rivel" },
            { 3, "Cloud" },
            { 4, "Galden" },
            { 5, "Folest" }
        };

        AssertWords(records, expectedValues);
    }

}
