using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

//IGNORE SPELLING: TRUNC untyped

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.DateTimeExpressions;

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
        new DateTime(2026, 9, 23, 6, 30, 15, DateTimeKind.Unspecified).AddTicks(1_234_560);

    private async Task<IEnumerable<DateTimeValue>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<Book> builder = new()
        {
            Selects = new SelectTags(new SelectTag(expression, "Value"))
        };
        SqlQuery query = _generator.Select(builder);
        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
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
            SharedDateTimePartEnum.Month => new DateTime(2026, 9, 1),
            SharedDateTimePartEnum.Week => new DateTime(2026, 9, 21),
            SharedDateTimePartEnum.Day => new DateTime(2026, 9, 23),
            SharedDateTimePartEnum.Hour => new DateTime(2026, 9, 23, 6, 0, 0),
            SharedDateTimePartEnum.Minute => new DateTime(2026, 9, 23, 6, 30, 0),
            SharedDateTimePartEnum.Second => new DateTime(2026, 9, 23, 6, 30, 15),
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
            DateTruncDateTimePartEnum.Microseconds => Value,
            DateTruncDateTimePartEnum.Milliseconds => new DateTime(2026, 9, 23, 6, 30, 15).AddTicks(1_230_000),
            DateTruncDateTimePartEnum.Second => new DateTime(2026, 9, 23, 6, 30, 15),
            DateTruncDateTimePartEnum.Minute => new DateTime(2026, 9, 23, 6, 30, 0),
            DateTruncDateTimePartEnum.Hour => new DateTime(2026, 9, 23, 6, 0, 0),
            DateTruncDateTimePartEnum.Day => new DateTime(2026, 9, 23),
            DateTruncDateTimePartEnum.Week => new DateTime(2026, 9, 21),
            DateTruncDateTimePartEnum.Month => new DateTime(2026, 9, 1),
            DateTruncDateTimePartEnum.Quarter => new DateTime(2026, 7, 1),
            DateTruncDateTimePartEnum.Year => new DateTime(2026, 1, 1),
            DateTruncDateTimePartEnum.Decade => new DateTime(2020, 1, 1),
            DateTruncDateTimePartEnum.Century => new DateTime(2001, 1, 1),
            DateTruncDateTimePartEnum.Millennium => new DateTime(2001, 1, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
        };

        DateTimeValue[] records = [.. await ExecuteAsync(new DateTrunc(datePart, new Parameter(Value)))];

        Assert.NotEmpty(records);
        Assert.All(records, record => Assert.Equal(expected, record.Value));
    }
}
