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

public sealed class DatePartTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public DatePartTests(BooksFixture fixture) => _fixture = fixture;

    public static IEnumerable<object[]> SharedDateParts =>
        Enum.GetValues<SharedDateTimePartEnum>().Select(static value => new object[] { value });

    public static IEnumerable<object[]> FunctionSpecificDateParts =>
        Enum.GetValues<DatePartDateTimePartEnum>().Select(static value => new object[] { value });

    private static DateTimeOffset Value =>
        new DateTimeOffset(2026, 9, 23, 6, 30, 15, 123, new TimeSpan(5, 30, 0)).AddTicks(4560);

    private async Task<IEnumerable<IntegerValue>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<Book> builder = new()
        {
            Selects = new SelectTags(new SelectTag(expression, "Value"))
        };
        SqlQuery query = _generator.Select(builder);
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        await connection.OpenAsync();
        await using SqlCommand setup = new("SET DATEFIRST 1;", connection);
        await setup.ExecuteNonQueryAsync();
        return await CommandsAsync.ExecuteReaderAsync<IntegerValue>(query, null, connection);
    }

    [Theory]
    [MemberData(nameof(SharedDateParts))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1042:The member referenced by the MemberData attribute returns untyped data rows", Justification = "<Pending>")]
    public async Task SharedEnumValue_Test(SharedDateTimePartEnum datePart)
    {
        int expected = datePart switch
        {
            SharedDateTimePartEnum.Year => 2026,
            SharedDateTimePartEnum.Quarter => 3,
            SharedDateTimePartEnum.Month => 9,
            SharedDateTimePartEnum.DayOfYear => 266,
            SharedDateTimePartEnum.Day => 23,
            SharedDateTimePartEnum.Week => 39,
            SharedDateTimePartEnum.Hour => 6,
            SharedDateTimePartEnum.Minute => 30,
            SharedDateTimePartEnum.Second => 15,
            SharedDateTimePartEnum.Millisecond => 123,
            SharedDateTimePartEnum.Microsecond => 123456,
            _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
        };

        IntegerValue[] records = [.. await ExecuteAsync(new DatePart(datePart, new Parameter(Value)))];

        Assert.NotEmpty(records);
        Assert.All(records, record => Assert.Equal(expected, record.Value));
    }

    [Theory]
    [MemberData(nameof(FunctionSpecificDateParts))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1042:The member referenced by the MemberData attribute returns untyped data rows", Justification = "<Pending>")]
    public async Task FunctionSpecificEnumValue_Test(DatePartDateTimePartEnum datePart)
    {
        int expected = datePart switch
        {
            DatePartDateTimePartEnum.Year => 2026,
            DatePartDateTimePartEnum.Quarter => 3,
            DatePartDateTimePartEnum.Month => 9,
            DatePartDateTimePartEnum.DayOfYear => 266,
            DatePartDateTimePartEnum.Day => 23,
            DatePartDateTimePartEnum.Week => 39,
            DatePartDateTimePartEnum.Weekday => 3,
            DatePartDateTimePartEnum.Hour => 6,
            DatePartDateTimePartEnum.Minute => 30,
            DatePartDateTimePartEnum.Second => 15,
            DatePartDateTimePartEnum.Millisecond => 123,
            DatePartDateTimePartEnum.Microsecond => 123456,
            DatePartDateTimePartEnum.Nanosecond => 123456000,
            DatePartDateTimePartEnum.IsoWeek => 39,
            DatePartDateTimePartEnum.TimezoneOffset => 330,
            _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
        };

        IntegerValue[] records = [.. await ExecuteAsync(new DatePart(datePart, new Parameter(Value)))];

        Assert.NotEmpty(records);
        Assert.All(records, record => Assert.Equal(expected, record.Value));
    }
}
