using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.DateTimeExpressions;

public sealed class EOMonthTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public EOMonthTests(BooksFixture fixture) => _fixture = fixture;

    private async Task<IEnumerable<DateOnlyValue>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<Book> builder = new()
        {
            Selects = new SelectTags(new SelectTag(expression, "Value"))
        };
        SqlQuery query = _generator.Select(builder);
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<DateOnlyValue>(query, null, connection);
    }

    [Fact]
    public async Task SingleExpressionConstructor_Test()
    {
        IEnumerable<DateOnlyValue> records = await ExecuteAsync(new EOMonth(new Parameter(new DateTime(2026, 9, 23))));

        Assert.All(records, record => Assert.Equal(new DateOnly(2026, 9, 30), record.Value));
    }

    [Fact]
    public async Task TwoExpressionConstructor_Test()
    {
        IEnumerable<DateOnlyValue> records = await ExecuteAsync(new EOMonth(new Parameter(new DateTime(2026, 9, 23)), new Parameter(1)));

        Assert.All(records, record => Assert.Equal(new DateOnly(2026, 10, 31), record.Value));
    }
}
