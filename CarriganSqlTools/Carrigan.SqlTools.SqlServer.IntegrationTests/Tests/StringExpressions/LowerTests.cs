using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.StringExpressions;

public sealed class LowerTests : IClassFixture<LeftRightFixture>
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();
    private readonly LeftRightFixture _fixture;
    private readonly SqlGenerator<LeftWords> LeftSqlGenerator = new();

    public LowerTests(LeftRightFixture fixture) =>
        _fixture = fixture;

    private async Task<IEnumerable<Words>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<LeftWords> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag<LeftWords>("Id"),
                new SelectTag(expression, "Word")
            )
        };

        SqlQuery query = LeftSqlGenerator.Select(selectBuilder);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
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
    public async Task Lower_Test()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Lower(new Column<LeftWords>(nameof(LeftWords.LeftWord)))
        );

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "apple" },
            { 2, "river" },
            { 3, "cloud" },
            { 4, "garden" },
            { 5, "forest" }
        };

        AssertWords(records, expectedValues);
    }
}
