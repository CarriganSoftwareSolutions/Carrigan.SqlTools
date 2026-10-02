using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests;

public sealed class BetweenTests : IClassFixture<SelectsFixture>
{
    private readonly SelectsFixture _fixture;
    private readonly SqlGenerator<Book> _bookSqlGenerator = new();

    public BetweenTests(SelectsFixture fixture) =>
        _fixture = fixture;

    [Fact]
    public async Task Between_IncludesBothBoundaryValues()
    {
        Predicates where = new Between
        (
            new Column<Book>(nameof(Book.YearPublished)),
            new Parameter(1865, "MinimumYear"),
            new Parameter(1890, "MaximumYear")
        );
        SelectBuilder<Book> selectBuilder = new()
        {
            Where = where
        };

        SqlQuery query = _bookSqlGenerator.Select(selectBuilder);
        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, connection);

        int[] expectedIds = [3, 4, 9];
        int[] actualIds = [.. books.Select(book => book.Id!.Value).OrderBy(id => id)];

        Assert.Equal(expectedIds, actualIds);
        foreach (int id in expectedIds)
            BookDataSet.ValidateById(books, id);
    }
}
