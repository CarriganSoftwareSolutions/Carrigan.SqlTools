using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.MathExpressions;

public sealed class PowerTests : IClassFixture<BooksFixture>
{
    private static readonly ISqlDialects Dialect = new PostgreSqlDialect();
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> BookSqlGenerator = new();

    public PowerTests(BooksFixture fixture) =>
        _fixture = fixture;

    private async Task<IEnumerable<BookIdAndPrice>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<Book> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag<Book>(nameof(Book.Id)),
                expression.AsSelectTag(nameof(BookIdAndPrice.Price))
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
                Assert.Equal(Math.Pow(expected.Value, 2.24), actual.Price.Value, 2, MidpointRounding.AwayFromZero);
            }
        }
    }

    [Fact]
    public async Task Power_Test()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Power(new Column<Book>(nameof(Book.Price)), 2.24)
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
}
