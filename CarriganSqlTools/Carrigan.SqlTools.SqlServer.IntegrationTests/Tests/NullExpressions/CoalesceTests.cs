using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.NullExpressions;

public sealed class CoalesceTests : IClassFixture<LeftRightFixture>
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();
    private readonly LeftRightFixture _fixture;
    private readonly SqlGenerator<Left> LeftSqlGenerator = new();

    public CoalesceTests(LeftRightFixture fixture) =>
        _fixture = fixture;

    private async Task<IEnumerable<Words>> ExecuteAsync(Coalesce coalesce)
    {
        ColumnEqualsColumn<Left, Right> ids = new (nameof(Left.Id), nameof(Right.Id));
        SelectBuilder<Left> selectBuilder = new()
        {
            Joins = new JoinTypes.FullJoin<Right>(ids),
            Selects = new SelectTags
            (
                new SelectTag(new Coalesce(new Column<Left>(nameof(Left.Id)), new Column<Right>(nameof(Right.Id))), "Id"),
                new SelectTag(coalesce, "Word")
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
    public async Task Coalesce_1()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Coalesce
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
            { 4, "Garden" },
            { 5, "Forest" },
            { 6, "Meadow" },
            { 7, "Lantern" },
            { 8, "Harbor" }
        };

        AssertWords(records, expectedValues);
    }

    [Fact]
    public async Task Coalesce_2()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Coalesce
            (
                new Column<Right>(nameof(Right.RightWord)),
                new Column<Left>(nameof(Left.LeftWord))
            )
        );

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "Apple" },
            { 2, "River" },
            { 3, "Cloud" },
            { 4, "Window" },
            { 5, "Bridge" },
            { 6, "Meadow" },
            { 7, "Lantern" },
            { 8, "Harbor" }
        };

        AssertWords(records, expectedValues);
    }

    [Fact]
    public async Task Coalesce_3()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Coalesce
            (
                new NullParameter<string>(Dialect),
                new Column<Right>(nameof(Right.RightWord)),
                new Column<Left>(nameof(Left.LeftWord))
            )
        );

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "Apple" },
            { 2, "River" },
            { 3, "Cloud" },
            { 4, "Window" },
            { 5, "Bridge" },
            { 6, "Meadow" },
            { 7, "Lantern" },
            { 8, "Harbor" }
        };

        AssertWords(records, expectedValues);
    }

    [Fact]
    public async Task Coalesce_4()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Coalesce
            (
                new Column<Right>(nameof(Right.RightWord)),
                new NullParameter<string>(Dialect),
                new Column<Left>(nameof(Left.LeftWord))
            )
        );

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "Apple" },
            { 2, "River" },
            { 3, "Cloud" },
            { 4, "Window" },
            { 5, "Bridge" },
            { 6, "Meadow" },
            { 7, "Lantern" },
            { 8, "Harbor" }
        };

        AssertWords(records, expectedValues);
    }


    [Fact]
    public async Task Coalesce_5()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Coalesce
            (
                new NullParameter<string>(Dialect, "NullParameter"),
                new Parameter("Hello"),
                new Column<Right>(nameof(Right.RightWord)),
                new Column<Left>(nameof(Left.LeftWord))
            )
        );

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "Hello" },
            { 2, "Hello" },
            { 3, "Hello" },
            { 4, "Hello" },
            { 5, "Hello" },
            { 6, "Hello" },
            { 7, "Hello" },
            { 8, "Hello" }
        };

        AssertWords(records, expectedValues);
    }


    [Fact]
    public async Task Coalesce_6()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Coalesce
            (
                new Parameter("Hello"),
                new NullParameter<string>(Dialect, "NullParameter"),
                new Column<Right>(nameof(Right.RightWord)),
                new Column<Left>(nameof(Left.LeftWord))
            )
        );

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "Hello" },
            { 2, "Hello" },
            { 3, "Hello" },
            { 4, "Hello" },
            { 5, "Hello" },
            { 6, "Hello" },
            { 7, "Hello" },
            { 8, "Hello" }
        };

        AssertWords(records, expectedValues);
    }

    [Fact]
    public async Task Coalesce_7()
    {
        IEnumerable<Words> records = await ExecuteAsync
        (
            new Coalesce
            (
                new NullParameter<string>(Dialect, "NullParameter"),
                new Column<Right>(nameof(Right.RightWord)),
                new Parameter("Hello"),
                new Column<Left>(nameof(Left.LeftWord))
            )
        );

        Dictionary<int, string?> expectedValues = new()
        {
            { 1, "Hello" },
            { 2, "Hello" },
            { 3, "Hello" },
            { 4, "Window" },
            { 5, "Bridge" },
            { 6, "Meadow" },
            { 7, "Lantern" },
            { 8, "Harbor" }
        };

        AssertWords(records, expectedValues);
    }
}
