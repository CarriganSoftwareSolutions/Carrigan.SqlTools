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

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests;

public sealed class ConcatTests : IClassFixture<LeftRightFixture>
{
    private static readonly ISqlDialects Dialect = new PostgreSqlDialect();
    private readonly LeftRightFixture _fixture;
    private readonly SqlGenerator<Left> LeftSqlGenerator = new();

    public ConcatTests(LeftRightFixture fixture) =>
        _fixture = fixture;

    private async Task<IEnumerable<Words>> ExecuteAsync(SqlExpression expression)
    {
        ColumnEqualsColumn<Left, Right> ids = new (nameof(Left.Id), nameof(Right.Id));
        SelectBuilder<Left> selectBuilder = new()
        {
            Joins = new JoinTypes.FullJoin<Right>(ids),
            Selects = new SelectTags
            (
                new SelectTag(new Coalesce(new Column<Left>(nameof(Left.Id)), new Column<Right>(nameof(Right.Id))), "Id"),
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
    public async Task Concat_Test()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Concat
            (
                new Column<Left>(nameof(Left.LeftWord)),
                new Column<Right>(nameof(Right.RightWord))
            )
        );

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "Apple" },
            { 2, "River" },
            { 3, "Cloud" },
            { 4, "GardenWindow" },
            { 5, "ForestBridge" },
            { 6, "Meadow" },
            { 7, "Lantern" },
            { 8, "Harbor" }
        };

        AssertWords(records, expectedValues);
    }
}
