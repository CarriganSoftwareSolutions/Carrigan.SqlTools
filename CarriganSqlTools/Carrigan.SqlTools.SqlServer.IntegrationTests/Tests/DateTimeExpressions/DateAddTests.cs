using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.DateTimeExpressions;

public sealed class DateAddTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public DateAddTests(BooksFixture fixture) => _fixture = fixture;

    public static IEnumerable<object[]> FunctionSpecificDateParts =>
        Enum.GetValues<DateAddDateTimePartEnum>().Select(static value => new object[] { value });

    private async Task<IEnumerable<DateTimeValue>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<Book> builder = new()
        {
            Selects = new SelectTags(new SelectTag(expression, "Value"))
        };
        SqlQuery query = _generator.Select(builder);
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<DateTimeValue>(query, null, connection);
    }

    [Fact]
    public async Task SharedAndFunctionSpecificConstructors_Test()
    {
        DateTime value = new(2026, 9, 23, 6, 30, 15);
        IEnumerable<DateTimeValue> shared = await ExecuteAsync(new DateAdd(SharedDateTimePartEnum.Day, new Parameter(2), new Parameter(value)));
        IEnumerable<DateTimeValue> specific = await ExecuteAsync(new DateAdd(DateAddDateTimePartEnum.Month, new Parameter(1), new Parameter(value)));

        Assert.All(shared, record => Assert.Equal(value.AddDays(2), record.Value));
        Assert.All(specific, record => Assert.Equal(value.AddMonths(1), record.Value));
    }

    [Theory]
    [MemberData(nameof(FunctionSpecificDateParts))]
    public async Task FunctionSpecificEnumValue_Test(DateAddDateTimePartEnum datePart)
    {
        DateTime value = new DateTime(2026, 9, 23, 6, 30, 15, 123).AddTicks(4567);
        int amount = datePart == DateAddDateTimePartEnum.Nanosecond ? 100 : 1;
        DateTimeValue[] records = [.. await ExecuteAsync(new DateAdd(datePart, new Parameter(amount), new Parameter(value)))];

        Assert.NotEmpty(records);
    }
}
