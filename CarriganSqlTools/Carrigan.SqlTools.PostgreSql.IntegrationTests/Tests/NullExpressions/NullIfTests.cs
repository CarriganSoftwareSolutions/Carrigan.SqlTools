using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.NullExpressions;

public sealed class NullIfTests : IClassFixture<LeftRightFixture>
{
    private static readonly ISqlDialects Dialect = new PostgreSqlDialect();
    private readonly LeftRightFixture _fixture;
    private readonly SqlGenerator<LeftWords> LeftSqlGenerator = new();

    public NullIfTests(LeftRightFixture fixture) =>
        _fixture = fixture;

    private async Task<IEnumerable<Words>> ExecuteAsync(NullIf nullIf)
    {
        SelectBuilder<LeftWords> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag<LeftWords>("Id"),
                new SelectTag(nullIf, "Word")
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
    public async Task NullIf_1()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new NullIf
            (
                new Column<LeftWords>(nameof(LeftWords.LeftWord)),
                new Parameter("Apple")
            )
        );

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, null },
            { 2, "River" },
            { 3, "Cloud" },
            { 4, "Garden" },
            { 5, "Forest" }
        };

        AssertWords(records, expectedValues);
    }

    [Fact]
    public async Task NullIf_2()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new NullIf
            (
                new Column<LeftWords>(nameof(LeftWords.LeftWord)),
                new Parameter("Cloud")
            )
        );

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "Apple" },
            { 2, "River" },
            { 3, null },
            { 4, "Garden" },
            { 5, "Forest" }
        };

        AssertWords(records, expectedValues);
    }

}
