using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.StringExpressions;

public sealed class TrimTests : IClassFixture<LeftRightFixture>
{
    private readonly LeftRightFixture _fixture;
    private readonly SqlGenerator<Left> LeftSqlGenerator = new();

    public TrimTests(LeftRightFixture fixture) =>
        _fixture = fixture;

    // Add the unwanted characters with CONCAT so the same fixture rows exercise each trim function.
    private static Concat CreatePaddedWord(string prefix, string suffix) =>
        new 
        (
            new Parameter(prefix),
            new Column<Left>(nameof(Left.LeftWord)),
            new Column<Right>(nameof(Right.RightWord)),
            new Parameter(suffix)
        );

    private async Task<IEnumerable<Words>> ExecuteAsync(SqlExpression expression)
    {
        ColumnEqualsColumn<Left, Right> ids = new(nameof(Left.Id), nameof(Right.Id));
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
            Assert.Equal(expected, actual.Word);
        }
    }

    [Fact]
    public async Task LTrim_RemovesLeadingSpaces()
    {
        SqlExpression expression = new LTrim(CreatePaddedWord("  ", "  "));
        IEnumerable<Words> records = await ExecuteAsync(expression);

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "Apple  " },
            { 2, "River  " },
            { 3, "Cloud  " },
            { 4, "GardenWindow  " },
            { 5, "ForestBridge  " },
            { 6, "Meadow  " },
            { 7, "Lantern  " },
            { 8, "Harbor  " }
        };

        AssertWords(records, expectedValues);
    }

    [Fact]
    public async Task RTrim_RemovesTrailingSpaces()
    {
        SqlExpression expression = new RTrim(CreatePaddedWord("  ", "  "));
        IEnumerable<Words> records = await ExecuteAsync(expression);

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "  Apple" },
            { 2, "  River" },
            { 3, "  Cloud" },
            { 4, "  GardenWindow" },
            { 5, "  ForestBridge" },
            { 6, "  Meadow" },
            { 7, "  Lantern" },
            { 8, "  Harbor" }
        };

        AssertWords(records, expectedValues);
    }

    [Fact]
    public async Task Trim_RemovesSpacesFromBothEnds()
    {
        SqlExpression expression = new Trim(CreatePaddedWord("  ", "  "));
        IEnumerable<Words> records = await ExecuteAsync(expression);

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

    [Fact]
    public async Task LTrim_RemovesSpecifiedLeadingCharacters_String()
    {
        SqlExpression expression = new LTrim(CreatePaddedWord("##~", "~##"), "#~");
        IEnumerable<Words> records = await ExecuteAsync(expression);

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "Apple~##" },
            { 2, "River~##" },
            { 3, "Cloud~##" },
            { 4, "GardenWindow~##" },
            { 5, "ForestBridge~##" },
            { 6, "Meadow~##" },
            { 7, "Lantern~##" },
            { 8, "Harbor~##" }
        };

        AssertWords(records, expectedValues);
    }

    [Fact]
    public async Task LTrim_RemovesSpecifiedLeadingCharacters_CharArray()
    {
        SqlExpression expression = new LTrim(CreatePaddedWord("##~", "~##"), ['#', '~']);
        IEnumerable<Words> records = await ExecuteAsync(expression);

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "Apple~##" },
            { 2, "River~##" },
            { 3, "Cloud~##" },
            { 4, "GardenWindow~##" },
            { 5, "ForestBridge~##" },
            { 6, "Meadow~##" },
            { 7, "Lantern~##" },
            { 8, "Harbor~##" }
        };

        AssertWords(records, expectedValues);
    }

    [Fact]
    public async Task RTrim_RemovesSpecifiedTrailingCharacters_String()
    {
        SqlExpression expression = new RTrim(CreatePaddedWord("##~", "~##"), "#~");
        IEnumerable<Words> records = await ExecuteAsync(expression);

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "##~Apple" },
            { 2, "##~River" },
            { 3, "##~Cloud" },
            { 4, "##~GardenWindow" },
            { 5, "##~ForestBridge" },
            { 6, "##~Meadow" },
            { 7, "##~Lantern" },
            { 8, "##~Harbor" }
        };

        AssertWords(records, expectedValues);
    }

    [Fact]
    public async Task RTrim_RemovesSpecifiedTrailingCharacters_CharArray()
    {
        SqlExpression expression = new RTrim(CreatePaddedWord("##~", "~##"), ['#', '~']);
        IEnumerable<Words> records = await ExecuteAsync(expression);

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "##~Apple" },
            { 2, "##~River" },
            { 3, "##~Cloud" },
            { 4, "##~GardenWindow" },
            { 5, "##~ForestBridge" },
            { 6, "##~Meadow" },
            { 7, "##~Lantern" },
            { 8, "##~Harbor" }
        };

        AssertWords(records, expectedValues);
    }

    [Fact]
    public async Task Trim_RemovesSpecifiedCharactersFromBothEnds_String()
    {
        SqlExpression expression = new Trim(CreatePaddedWord("##~", "~##"), "#~");
        IEnumerable<Words> records = await ExecuteAsync(expression);

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

    [Fact]
    public async Task Trim_RemovesSpecifiedCharactersFromBothEnds_CharArray()
    {
        SqlExpression expression = new Trim(CreatePaddedWord("##~", "~##"), ['#', '~']);
        IEnumerable<Words> records = await ExecuteAsync(expression);

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
