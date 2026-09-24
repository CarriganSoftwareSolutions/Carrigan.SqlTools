using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

//IGNORE SPELLING: Substring, ppl, ive, ard lou

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.StringExpressions;

public sealed class SplitPartTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public SplitPartTests(BooksFixture fixture) =>
        _fixture = fixture;

    private async Task<IEnumerable<BookIdAndTitle>> ExecuteAsync(SqlExpression expression, Predicates where)
    {
        SelectBuilder<Book> selectBuilder = new()
        {
            Selects = new SelectTags([new Column<Book>(nameof(Book.Id)).AsSelectTag("Id"), expression.AsSelectTag(nameof(BookIdAndTitle.Title))]),
            Where = where
        };

        SqlQuery query = _generator.Select(selectBuilder);

        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<BookIdAndTitle>(query, null, connection);
    }

    private static void AssertTitleParts(IEnumerable<BookIdAndTitle> records, Dictionary<int, string> expected)
    {
        BookIdAndTitle[] actual = [.. records];
        Assert.Equal(expected.Count, actual.Length);

        foreach (BookIdAndTitle record in actual)
        {
            Assert.True(expected.TryGetValue(record.Id, out string? value));
            Assert.Equal(value, record.Title);
        }
    }

    [Fact]
    public async Task SplitPart_ConstantConstructor_Test()
    {
        SqlExpression expression = new SplitPart(new Column<Book>(nameof(Book.Title)), " ", 3);
        Predicates predicates = new ColumnValue<Book>(nameof(Book.Id), 3);
        IEnumerable<BookIdAndTitle> records = await ExecuteAsync(expression, predicates);

        AssertTitleParts(records, new Dictionary<int, string>
        {
            { 3, "Huckleberry" }
        });
    }

    [Fact]
    public async Task SplitPart_ExpressionConstructor_Test()
    {
        SqlExpression expression = new SplitPart(new Column<Book>(nameof(Book.Title)), new Parameter(" "), new Parameter(3));
        Predicates predicates = new ColumnValue<Book>(nameof(Book.Id), 1);
        IEnumerable<BookIdAndTitle> records = await ExecuteAsync(expression, predicates);

        AssertTitleParts(records, new Dictionary<int, string>
        {
            { 1, "Prejudice" }
        });
    }
}
