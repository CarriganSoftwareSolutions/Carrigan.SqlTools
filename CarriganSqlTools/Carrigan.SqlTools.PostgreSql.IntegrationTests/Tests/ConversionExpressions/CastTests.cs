using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Npgsql;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.ConversionExpressions;

public sealed class TryCastTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public TryCastTests(BooksFixture fixture) =>
        _fixture = fixture;

    private async Task<IEnumerable<IntegerValue>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<Book> builder = new()
        {
            Selects = new SelectTags(new SelectTag(expression, nameof(IntegerValue.Value)))
        };

        SqlQuery query = _generator.Select(builder);

        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<IntegerValue>(query, null, connection);
    }

    [Fact]
    public async Task ValidConversion_ReturnsConvertedValue()
    {
        IEnumerable<IntegerValue> records = await ExecuteAsync
        (
            new Cast(new Parameter("123"), PostgreSqlTypesProvider.AsInteger(false, true))
        );

        Assert.All(records, record => Assert.Equal(123, record.Value));
    }
}
