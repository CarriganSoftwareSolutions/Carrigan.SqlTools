using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.ConversionExpressions;

public sealed class TryCastTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public TryCastTests(BooksFixture fixture) =>
        _fixture = fixture;

    private async Task<IEnumerable<NullableIntegerValue>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<Book> builder = new()
        {
            Selects = new SelectTags(new SelectTag(expression, nameof(NullableIntegerValue.Value)))
        };

        SqlQuery query = _generator.Select(builder);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<NullableIntegerValue>(query, null, connection);
    }

    [Fact]
    public async Task ValidConversion_ReturnsConvertedValue()
    {
        IEnumerable<NullableIntegerValue> records = await ExecuteAsync
        (
            new TryCast(new Parameter("123"), SqlServerTypesProvider.AsInt(true))
        );

        Assert.All(records, record => Assert.Equal(123, record.Value));
    }

    [Fact]
    public async Task MalformedValue_ReturnsNull()
    {
        IEnumerable<NullableIntegerValue> records = await ExecuteAsync
        (
            new TryCast(new Parameter("not-an-integer"), SqlServerTypesProvider.AsInt(true))
        );

        Assert.All(records, record => Assert.Null(record.Value));
    }

    public sealed class NullableIntegerValue
    {
        public int? Value { get; set; }
    }
}