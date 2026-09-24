using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

//IGNORE SPELLING: TRUNC, untyped

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.DateTimeExpressions;

public sealed class DateTruncTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public DateTruncTests(BooksFixture fixture) => _fixture = fixture;

    public static IEnumerable<object[]> SharedDateParts =>
        Enum.GetValues<SharedDateTimePartEnum>().Select(static value => new object[] { value });

    public static IEnumerable<object[]> FunctionSpecificDateParts =>
        Enum.GetValues<DateTruncDateTimePartEnum>().Select(static value => new object[] { value });

    private static DateTime Value =>
        new DateTime(2026, 9, 23, 6, 30, 15).AddTicks(1_234_567);

    private async Task<IEnumerable<DateTimeValue>> ExecuteAsync(SqlExpression expression)
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
        return await CommandsAsync.ExecuteReaderAsync<DateTimeValue>(query, null, connection);
    }

    [Theory]
    [MemberData(nameof(SharedDateParts))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1042:The member referenced by the MemberData attribute returns untyped data rows", Justification = "<Pending>")]
    public async Task SharedEnumValue_Test(SharedDateTimePartEnum datePart)
    {
        DateTime expected = datePart switch
        {
            SharedDateTimePartEnum.Year => new DateTime(2026, 1, 1),
            SharedDateTimePartEnum.Quarter => new DateTime(2026, 7, 1),
            SharedDateTimePartEnum.Month => new DateTime(2026, 9, 1),
            SharedDateTimePartEnum.DayOfYear => new DateTime(2026, 9, 23),
            SharedDateTimePartEnum.Day => new DateTime(2026, 9, 23),
            SharedDateTimePartEnum.Week => new DateTime(2026, 9, 21),
            SharedDateTimePartEnum.Hour => new DateTime(2026, 9, 23, 6, 0, 0),
            SharedDateTimePartEnum.Minute => new DateTime(2026, 9, 23, 6, 30, 0),
            SharedDateTimePartEnum.Second => new DateTime(2026, 9, 23, 6, 30, 15),
            SharedDateTimePartEnum.Millisecond => new DateTime(2026, 9, 23, 6, 30, 15).AddTicks(1_230_000),
            SharedDateTimePartEnum.Microsecond => new DateTime(2026, 9, 23, 6, 30, 15).AddTicks(1_234_560),
            _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
        };

        DateTimeValue[] records = [.. await ExecuteAsync(new DateTrunc(datePart, new Parameter(Value)))];

        Assert.NotEmpty(records);
        Assert.All(records, record => Assert.Equal(expected, record.Value));
    }

    [Theory]
    [MemberData(nameof(FunctionSpecificDateParts))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1042:The member referenced by the MemberData attribute returns untyped data rows", Justification = "<Pending>")]
    public async Task FunctionSpecificEnumValue_Test(DateTruncDateTimePartEnum datePart)
    {
        DateTime expected = datePart switch
        {
            DateTruncDateTimePartEnum.Year => new DateTime(2026, 1, 1),
            DateTruncDateTimePartEnum.Quarter => new DateTime(2026, 7, 1),
            DateTruncDateTimePartEnum.Month => new DateTime(2026, 9, 1),
            DateTruncDateTimePartEnum.DayOfYear => new DateTime(2026, 9, 23),
            DateTruncDateTimePartEnum.Day => new DateTime(2026, 9, 23),
            DateTruncDateTimePartEnum.Week => new DateTime(2026, 9, 21),
            DateTruncDateTimePartEnum.IsoWeek => new DateTime(2026, 9, 21),
            DateTruncDateTimePartEnum.Hour => new DateTime(2026, 9, 23, 6, 0, 0),
            DateTruncDateTimePartEnum.Minute => new DateTime(2026, 9, 23, 6, 30, 0),
            DateTruncDateTimePartEnum.Second => new DateTime(2026, 9, 23, 6, 30, 15),
            DateTruncDateTimePartEnum.Millisecond => new DateTime(2026, 9, 23, 6, 30, 15).AddTicks(1_230_000),
            DateTruncDateTimePartEnum.Microsecond => new DateTime(2026, 9, 23, 6, 30, 15).AddTicks(1_234_560),
            _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
        };

        DateTimeValue[] records = [.. await ExecuteAsync(new DateTrunc(datePart, new Parameter(Value)))];

        Assert.NotEmpty(records);
        Assert.All(records, record => Assert.Equal(expected, record.Value));
    }
}
