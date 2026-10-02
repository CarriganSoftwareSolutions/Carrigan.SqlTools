using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests;

public sealed class NotBetweenTests : IClassFixture<SelectsFixture>
{
    private readonly SelectsFixture _fixture;
    private readonly SqlGenerator<Book> _bookSqlGenerator = new();

    public NotBetweenTests(SelectsFixture fixture) =>
        _fixture = fixture;

    [Fact]
    public async Task NotBetween_ExcludesBothBoundaryValues()
    {
        Predicates where = new NotBetween
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
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, connection);

        int[] expectedIds = [1, 2, 5, 6, 7, 8, 10, 11];
        int[] actualIds = [.. books.Select(book => book.Id!.Value).OrderBy(id => id)];

        Assert.Equal(expectedIds, actualIds);
        foreach (int id in expectedIds)
            BookDataSet.ValidateById(books, id);
    }
}
