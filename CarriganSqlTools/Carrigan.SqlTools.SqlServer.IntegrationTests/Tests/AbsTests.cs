using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests;

public sealed class RoundTests : IClassFixture<BooksFixture>
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> BookSqlGenerator = new();

    public RoundTests(BooksFixture fixture) =>
        _fixture = fixture;

    private async Task<IEnumerable<BookIdAndPrice>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<Book> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag<Book>(nameof(Book.Id)),
                new SelectTag(expression, nameof(BookIdAndPrice.Price))
            )
        };

        SqlQuery query = BookSqlGenerator.Select(selectBuilder);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<BookIdAndPrice>(query, null, connection);
    }
    private static void AssertPrices(IEnumerable<BookIdAndPrice> records, Dictionary<int, double?> expectedValues)
    {
        BookIdAndPrice[] actualRecords = [.. records];

        Assert.Equal(expectedValues.Count, actualRecords.Length);

        foreach (BookIdAndPrice actual in actualRecords)
        {
            Assert.True(expectedValues.TryGetValue(actual.Id, out double? expected));

            if (expected is null)
            {
                Assert.Null(actual.Price);
            }
            else
            {
                Assert.NotNull(actual.Price);
                Assert.Equal(expected.Value, actual.Price.Value, 2, MidpointRounding.AwayFromZero);
            }
        }
    }

    [Fact]
    public async Task Round_3_Test()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Round ( new Column<Book>(nameof(Book.Price)), 3 )
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, 14.99 },
            { 2, 18.99 },
            { 3, 12.99 },
            { 4, 9.99 },
            { 5, 11.99 },
            { 6, 10.99 },
            { 7, 13.99 },
            { 8, 8.99 },
            { 9, 10.99 },
            { 10, 19.99 },
            { 11, null }
        };

        AssertPrices(records, expectedValues);
    }

    [Fact]
    public async Task Round_2_Test()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Round(new Column<Book>(nameof(Book.Price)), 2)
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, 14.99 },
            { 2, 18.99 },
            { 3, 12.99 },
            { 4, 9.99 },
            { 5, 11.99 },
            { 6, 10.99 },
            { 7, 13.99 },
            { 8, 8.99 },
            { 9, 10.99 },
            { 10, 19.99 },
            { 11, null }
        };

        AssertPrices(records, expectedValues);
    }

    [Fact]
    public async Task Round_1_Test()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Round(new Column<Book>(nameof(Book.Price)), 1)
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, 15 },
            { 2, 19 },
            { 3, 13 },
            { 4, 10 },
            { 5, 12 },
            { 6, 11 },
            { 7, 14 },
            { 8, 9 },
            { 9, 11 },
            { 10, 20 },
            { 11, null }
        };

        AssertPrices(records, expectedValues);
    }

    [Fact]
    public async Task Round_0_Test()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Round(new Column<Book>(nameof(Book.Price)), 0)
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, 15 },
            { 2, 19 },
            { 3, 13 },
            { 4, 10 },
            { 5, 12 },
            { 6, 11 },
            { 7, 14 },
            { 8, 9 },
            { 9, 11 },
            { 10, 20 },
            { 11, null }
        };

        AssertPrices(records, expectedValues);
    }

    [Fact]
    public async Task Round_negative_1_Test()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Round(new Column<Book>(nameof(Book.Price)), -1)
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, 10 },
            { 2, 20 },
            { 3, 10 },
            { 4, 10 },
            { 5, 10 },
            { 6, 10 },
            { 7, 10 },
            { 8, 10 },
            { 9, 10 },
            { 10, 20 },
            { 11, null }
        };

        AssertPrices(records, expectedValues);
    }
}
