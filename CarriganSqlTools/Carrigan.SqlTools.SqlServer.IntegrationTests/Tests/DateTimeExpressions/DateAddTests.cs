using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

//IGNORE SPELLING: untyped

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.DateTimeExpressions;

public sealed class DateAddTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public DateAddTests(BooksFixture fixture) => _fixture = fixture;

    public static IEnumerable<object[]> SharedDateParts =>
        Enum.GetValues<SharedDateTimePartEnum>().Select(static value => new object[] { value });

    public static IEnumerable<object[]> FunctionSpecificDateParts =>
        Enum.GetValues<DateAddDateTimePartEnum>().Select(static value => new object[] { value });

    private static DateTime Value =>
        new DateTime(2026, 9, 23, 6, 30, 15).AddTicks(1_234_560);

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

    [Theory]
    [MemberData(nameof(SharedDateParts))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1042:The member referenced by the MemberData attribute returns untyped data rows", Justification = "<Pending>")]
    public async Task SharedEnumValue_Test(SharedDateTimePartEnum datePart)
    {
        DateTime expected = datePart switch
        {
            SharedDateTimePartEnum.Year => Value.AddYears(1),
            SharedDateTimePartEnum.Quarter => Value.AddMonths(3),
            SharedDateTimePartEnum.Month => Value.AddMonths(1),
            SharedDateTimePartEnum.DayOfYear => Value.AddDays(1),
            SharedDateTimePartEnum.Day => Value.AddDays(1),
            SharedDateTimePartEnum.Week => Value.AddDays(7),
            SharedDateTimePartEnum.Hour => Value.AddHours(1),
            SharedDateTimePartEnum.Minute => Value.AddMinutes(1),
            SharedDateTimePartEnum.Second => Value.AddSeconds(1),
            SharedDateTimePartEnum.Millisecond => Value.AddMilliseconds(1),
            SharedDateTimePartEnum.Microsecond => Value.AddTicks(10),
            _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
        };

        DateTimeValue[] records = [.. await ExecuteAsync(new DateAdd(datePart, new Parameter(1), new Parameter(Value)))];

        Assert.NotEmpty(records);
        Assert.All(records, record => Assert.Equal(expected, record.Value));
    }

    [Theory]
    [MemberData(nameof(FunctionSpecificDateParts))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1042:The member referenced by the MemberData attribute returns untyped data rows", Justification = "<Pending>")]
    public async Task FunctionSpecificEnumValue_Test(DateAddDateTimePartEnum datePart)
    {
        int amount = datePart == DateAddDateTimePartEnum.Nanosecond ? 100 : 1;
        DateTime expected = datePart switch
        {
            DateAddDateTimePartEnum.Year => Value.AddYears(1),
            DateAddDateTimePartEnum.Quarter => Value.AddMonths(3),
            DateAddDateTimePartEnum.Month => Value.AddMonths(1),
            DateAddDateTimePartEnum.DayOfYear => Value.AddDays(1),
            DateAddDateTimePartEnum.Day => Value.AddDays(1),
            DateAddDateTimePartEnum.Week => Value.AddDays(7),
            DateAddDateTimePartEnum.Weekday => Value.AddDays(1),
            DateAddDateTimePartEnum.Hour => Value.AddHours(1),
            DateAddDateTimePartEnum.Minute => Value.AddMinutes(1),
            DateAddDateTimePartEnum.Second => Value.AddSeconds(1),
            DateAddDateTimePartEnum.Millisecond => Value.AddMilliseconds(1),
            DateAddDateTimePartEnum.Microsecond => Value.AddTicks(10),
            DateAddDateTimePartEnum.Nanosecond => Value.AddTicks(1),
            _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
        };

        DateTimeValue[] records = [.. await ExecuteAsync(new DateAdd(datePart, new Parameter(amount), new Parameter(Value)))];

        Assert.NotEmpty(records);
        Assert.All(records, record => Assert.Equal(expected, record.Value));
    }
}
