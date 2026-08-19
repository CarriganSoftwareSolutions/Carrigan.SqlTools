using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests;

public sealed class ArithmeticTests : IClassFixture<ArithmeticFixture>
{
    private readonly ArithmeticFixture _fixture;
    private readonly SqlGenerator<Book> BookSqlGenerator = new();

    public ArithmeticTests(ArithmeticFixture fixture) =>
        _fixture = fixture;

    [Fact]
    public async Task Add()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Add
            (
                new NumericColumn<Book>(nameof(Book.Price)),
                new Parameter(1.1),
                new Parameter(2.2)
            )
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, 18.29 },
            { 2, 22.29 },
            { 3, 16.29 },
            { 4, 13.29 },
            { 5, 15.29 },
            { 6, 14.29 },
            { 7, 17.29 },
            { 8, 12.29 },
            { 9, 14.29 },
            { 10, 23.29 },
            { 11, null },
        };

        AssertPrices(records, expectedValues);
    }

    [Fact]
    public async Task Subtract()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Subtract
            (
                new NumericColumn<Book>(nameof(Book.Price)),
                new Parameter(1.1),
                new Parameter(2.2)
            )
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, 11.69 },
            { 2, 15.69 },
            { 3, 9.69 },
            { 4, 6.69 },
            { 5, 8.69 },
            { 6, 7.69 },
            { 7, 10.69 },
            { 8, 5.69 },
            { 9, 7.69 },
            { 10, 16.69 },
            { 11, null },
        };

        AssertPrices(records, expectedValues);
    }

    [Fact]
    public async Task Minus()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Minus
            (
                new NumericColumn<Book>(nameof(Book.Price)),
                new Parameter(1.1),
                new Parameter(2.2)
            )
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, 11.69 },
            { 2, 15.69 },
            { 3, 9.69 },
            { 4, 6.69 },
            { 5, 8.69 },
            { 6, 7.69 },
            { 7, 10.69 },
            { 8, 5.69 },
            { 9, 7.69 },
            { 10, 16.69 },
            { 11, null },
        };

        AssertPrices(records, expectedValues);
    }

    [Fact]
    public async Task Multiply()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Multiply
            (
                new NumericColumn<Book>(nameof(Book.Price)),
                new Parameter(2.0),
                new Parameter(3.0)
            )
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, 89.94 },
            { 2, 113.94 },
            { 3, 77.94 },
            { 4, 59.94 },
            { 5, 71.94 },
            { 6, 65.94 },
            { 7, 83.94 },
            { 8, 53.94 },
            { 9, 65.94 },
            { 10, 119.94 },
            { 11, null },
        };

        AssertPrices(records, expectedValues);
    }

    [Fact]
    public async Task Divide()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Divide
            (
                new NumericColumn<Book>(nameof(Book.Price)),
                new Parameter(2.0),
                new Parameter(3.0)
            )
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, 2.498333333 },
            { 2, 3.1649999999999996 },
            { 3, 2.165 },
            { 4, 1.665 },
            { 5, 1.998333333 },
            { 6, 1.831666666 },
            { 7, 2.331666666 },
            { 8, 1.498333333 },
            { 9, 1.831666666 },
            { 10, 3.331666666 },
            { 11, null },
        };

        AssertPrices(records, expectedValues);
    }

    [Fact]
    public async Task Mod()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Mod
            (
                new NumericColumn<Book>(nameof(Book.Price)),
                new Parameter(8),
                new Parameter(5)
            )
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, 1.99 },
            { 2, 2.99 },
            { 3, 4.99 },
            { 4, 1.99 },
            { 5, 3.99 },
            { 6, 2.99 },
            { 7, 0.99 },
            { 8, 0.99 },
            { 9, 2.99 },
            { 10, 3.99 },
            { 11, null },
        };

        AssertPrices(records, expectedValues);
    }

    [Fact]
    public async Task Modulo()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Modulo
            (
                new NumericColumn<Book>(nameof(Book.Price)),
                new Parameter(8),
                new Parameter(5)
            )
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, 1.99 },
            { 2, 2.99 },
            { 3, 4.99 },
            { 4, 1.99 },
            { 5, 3.99 },
            { 6, 2.99 },
            { 7, 0.99 },
            { 8, 0.99 },
            { 9, 2.99 },
            { 10, 3.99 },
            { 11, null },
        };

        AssertPrices(records, expectedValues);
    }

    [Fact]
    public async Task Negate()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Negate
            (
                new NumericColumn<Book>(nameof(Book.Price))
            )
        );

        Dictionary<int, double?> expectedValues = new()
        {
            { 1, -14.99 },
            { 2, -18.99 },
            { 3, -12.99 },
            { 4, -9.99 },
            { 5, -11.99 },
            { 6, -10.99 },
            { 7, -13.99 },
            { 8, -8.99 },
            { 9, -10.99 },
            { 10, -19.99 },
            { 11, null },
        };

        AssertPrices(records, expectedValues);
    }

    private async Task<IEnumerable<BookIdAndPrice>> ExecuteAsync(NumericExpression expression)
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

        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
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
}
