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

public sealed class CeilingTests : IClassFixture<BooksFixture>
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> BookSqlGenerator = new();

    public CeilingTests(BooksFixture fixture) =>
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
    public async Task Ceiling()
    {
        IEnumerable<BookIdAndPrice> records = await ExecuteAsync
        (
            new Ceiling(new Column<Book>(nameof(Book.Price)))
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
}
