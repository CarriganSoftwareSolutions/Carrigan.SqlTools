using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.DateTimeExpressions;

public sealed class MakeDateTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public MakeDateTests(BooksFixture fixture) => _fixture = fixture;

    private async Task<IEnumerable<DateOnlyValue>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<Book> builder = new()
        {
            Selects = new SelectTags(new SelectTag(expression, "Value"))
        };
        SqlQuery query = _generator.Select(builder);
        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<DateOnlyValue>(query, null, connection);
    }

    [Fact]
    public async Task ExpressionConstructor_Test()
    {
        IEnumerable<DateOnlyValue> records = await ExecuteAsync
        (
            new MakeDate(new Parameter(2026), new Parameter(9), new Parameter(23))
        );

        Assert.All(records, record => Assert.Equal(new DateOnly(2026, 9, 23), record.Value));
    }

    [Fact]
    public async Task IntegerConstructor_Test()
    {
        IEnumerable<DateOnlyValue> records = await ExecuteAsync(new MakeDate(2026, 9, 23));

        Assert.All(records, record => Assert.Equal(new DateOnly(2026, 9, 23), record.Value));
    }
}
