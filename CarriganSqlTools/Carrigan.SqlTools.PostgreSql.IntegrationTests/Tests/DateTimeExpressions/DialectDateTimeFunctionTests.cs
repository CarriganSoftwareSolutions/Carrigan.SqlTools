using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.DateTimeExpressions;

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
        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<T>(query, null, connection);
    }

    [Fact]
    public async Task Extract_SharedAndFunctionSpecificConstructors_Test()
    {
        DateTime value = new(2026, 9, 23, 6, 30, 15);
        IEnumerable<DecimalValue> shared = await ExecuteAsync<DecimalValue>(new Extract(SharedDateTimePartEnum.Day, new Parameter(value)));
        IEnumerable<DecimalValue> specific = await ExecuteAsync<DecimalValue>(new Extract(ExtractDateTimePartEnum.Month, new Parameter(value)));
        Assert.All(shared, record => Assert.Equal(23m, record.Value));
        Assert.All(specific, record => Assert.Equal(9m, record.Value));
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
    public async Task MakeInterval_SharedAndFunctionSpecificConstructors_Test()
    {
        IEnumerable<TimeSpanValue> shared = await ExecuteAsync<TimeSpanValue>(new MakeInterval(SharedDateTimePartEnum.Day, new Parameter(2)));
        IEnumerable<TimeSpanValue> specific = await ExecuteAsync<TimeSpanValue>(new MakeInterval(MakeIntervalDateTimePartEnum.Hour, new Parameter(3)));
        Assert.All(shared, record => Assert.Equal(TimeSpan.FromDays(2), record.Value));
        Assert.All(specific, record => Assert.Equal(TimeSpan.FromHours(3), record.Value));
    }

    [Fact]
    public async Task MakeDate_Test()
    {
        IEnumerable<DateOnlyValue> records = await ExecuteAsync<DateOnlyValue>(new MakeDate(new Parameter(2026), new Parameter(9), new Parameter(23)));
        Assert.All(records, record => Assert.Equal(new DateOnly(2026, 9, 23), record.Value));
    }
}
