using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.DateTimeExpressions;

public sealed class DialectDateTimeFunctionTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public DialectDateTimeFunctionTests(BooksFixture fixture) => _fixture = fixture;

    private async Task<IEnumerable<T>> ExecuteAsync<T>(SqlExpression expression) where T : class, new()
    {
        SelectBuilder<Book> builder = new()
        {
            Selects = new SelectTags(new SelectTag(expression, "Value"))
        };
        SqlQuery query = _generator.Select(builder);
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<T>(query, null, connection);
    }

    [Fact]
    public async Task DatePart_SharedAndFunctionSpecificConstructors_Test()
    {
        DateTime value = new(2026, 9, 23, 6, 30, 15);
        IEnumerable<IntegerValue> shared = await ExecuteAsync<IntegerValue>(new DatePart(SharedDateTimePartEnum.Day, new Parameter(value)));
        IEnumerable<IntegerValue> specific = await ExecuteAsync<IntegerValue>(new DatePart(DatePartDateTimePartEnum.Month, new Parameter(value)));
        Assert.All(shared, record => Assert.Equal(23, record.Value));
        Assert.All(specific, record => Assert.Equal(9, record.Value));
    }

    [Fact]
    public async Task DateAdd_SharedAndFunctionSpecificConstructors_Test()
    {
        DateTime value = new(2026, 9, 23, 6, 30, 15);
        IEnumerable<DateTimeValue> shared = await ExecuteAsync<DateTimeValue>(new DateAdd(SharedDateTimePartEnum.Day, new Parameter(2), new Parameter(value)));
        IEnumerable<DateTimeValue> specific = await ExecuteAsync<DateTimeValue>(new DateAdd(DateAddDateTimePartEnum.Month, new Parameter(1), new Parameter(value)));
        Assert.All(shared, record => Assert.Equal(value.AddDays(2), record.Value));
        Assert.All(specific, record => Assert.Equal(value.AddMonths(1), record.Value));
    }

    [Fact]
    public async Task DateTrunc_SharedAndFunctionSpecificConstructors_Test()
    {
        DateTime value = new(2026, 9, 23, 6, 30, 15);
        IEnumerable<DateTimeValue> shared = await ExecuteAsync<DateTimeValue>(new DateTrunc(SharedDateTimePartEnum.Day, new Parameter(value)));
        IEnumerable<DateTimeValue> specific = await ExecuteAsync<DateTimeValue>(new DateTrunc(DateTruncDateTimePartEnum.Month, new Parameter(value)));
        Assert.All(shared, record => Assert.Equal(new DateTime(2026, 9, 23), record.Value));
        Assert.All(specific, record => Assert.Equal(new DateTime(2026, 9, 1), record.Value));
    }

    [Fact]
    public async Task DateFromParts_Test()
    {
        IEnumerable<DateOnlyValue> records = await ExecuteAsync<DateOnlyValue>(new DateFromParts(new Parameter(2026), new Parameter(9), new Parameter(23)));
        Assert.All(records, record => Assert.Equal(new DateOnly(2026, 9, 23), record.Value));
    }

    [Fact]
    public async Task EOMonth_Test()
    {
        IEnumerable<DateOnlyValue> records = await ExecuteAsync<DateOnlyValue>(new EOMonth(new Parameter(new DateTime(2026, 9, 23))));
        Assert.All(records, record => Assert.Equal(new DateOnly(2026, 9, 30), record.Value));
    }
}
