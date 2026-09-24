using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

//IGNORE SPELLING: Kolkata untyped

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.DateTimeExpressions;

public sealed class ExtractTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public ExtractTests(BooksFixture fixture) => _fixture = fixture;

    public static IEnumerable<object[]> SharedDateParts =>
        Enum.GetValues<SharedDateTimePartEnum>().Select(static value => new object[] { value });

    public static IEnumerable<object[]> FunctionSpecificDateParts =>
        Enum.GetValues<ExtractDateTimePartEnum>().Select(static value => new object[] { value });

    private static DateTimeOffset Value =>
        new DateTimeOffset(2026, 9, 23, 12, 30, 15, TimeSpan.Zero).AddTicks(1_234_560);

    private static DateTimeOffset JulianValue =>
        new(2000, 1, 1, 12, 0, 0, TimeSpan.Zero);

    private async Task<IEnumerable<DecimalValue>> ExecuteAsync(SqlExpression expression, string timeZone = "UTC")
    {
        SelectBuilder<Book> builder = new()
        {
            Selects = new SelectTags(new SelectTag(expression, "Value"))
        };
        SqlQuery query = _generator.Select(builder);
        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
        await connection.OpenAsync();
        await using NpgsqlCommand setup = new($"SET TIME ZONE '{timeZone}';", connection);
        await setup.ExecuteNonQueryAsync();
        return await CommandsAsync.ExecuteReaderAsync<DecimalValue>(query, null, connection);
    }

    [Theory]
    [MemberData(nameof(SharedDateParts))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1042:The member referenced by the MemberData attribute returns untyped data rows", Justification = "<Pending>")]
    public async Task SharedEnumValue_Test(SharedDateTimePartEnum datePart)
    {
        decimal expected = datePart switch
        {
            SharedDateTimePartEnum.Year => 2026m,
            SharedDateTimePartEnum.Month => 9m,
            SharedDateTimePartEnum.Week => 39m,
            SharedDateTimePartEnum.Day => 23m,
            SharedDateTimePartEnum.Hour => 12m,
            SharedDateTimePartEnum.Minute => 30m,
            SharedDateTimePartEnum.Second => 15.123456m,
            _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
        };

        DecimalValue[] records = [.. await ExecuteAsync(new Extract(datePart, new Parameter(Value)))];

        Assert.NotEmpty(records);
        Assert.All(records, record => Assert.Equal(expected, record.Value));
    }

    [Theory]
    [MemberData(nameof(FunctionSpecificDateParts))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1042:The member referenced by the MemberData attribute returns untyped data rows", Justification = "<Pending>")]
    public async Task FunctionSpecificEnumValue_Test(ExtractDateTimePartEnum datePart)
    {
        object value = datePart == ExtractDateTimePartEnum.Julian ? JulianValue : Value;
        string timeZone = IsTimeZonePart(datePart) ? "Asia/Kolkata" : "UTC";
        decimal expected = datePart switch
        {
            ExtractDateTimePartEnum.Century => 21m,
            ExtractDateTimePartEnum.Day => 23m,
            ExtractDateTimePartEnum.Decade => 202m,
            ExtractDateTimePartEnum.DayOfWeek => 3m,
            ExtractDateTimePartEnum.DayOfYear => 266m,
            ExtractDateTimePartEnum.Epoch => 1790166615.123456m,
            ExtractDateTimePartEnum.Hour => 12m,
            ExtractDateTimePartEnum.IsoDayOfWeek => 3m,
            ExtractDateTimePartEnum.IsoYear => 2026m,
            ExtractDateTimePartEnum.Julian => 2451545.5m,
            ExtractDateTimePartEnum.Microseconds => 15123456m,
            ExtractDateTimePartEnum.Millennium => 3m,
            ExtractDateTimePartEnum.Milliseconds => 15123.456m,
            ExtractDateTimePartEnum.Minute => 30m,
            ExtractDateTimePartEnum.Month => 9m,
            ExtractDateTimePartEnum.Quarter => 3m,
            ExtractDateTimePartEnum.Second => 15.123456m,
            ExtractDateTimePartEnum.Timezone => 19800m,
            ExtractDateTimePartEnum.TimezoneHour => 5m,
            ExtractDateTimePartEnum.TimezoneMinute => 30m,
            ExtractDateTimePartEnum.Week => 39m,
            ExtractDateTimePartEnum.Year => 2026m,
            _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
        };

        DecimalValue[] records = [.. await ExecuteAsync(new Extract(datePart, new Parameter(value)), timeZone)];

        Assert.NotEmpty(records);
        Assert.All(records, record => Assert.Equal(expected, record.Value));
    }

    private static bool IsTimeZonePart(ExtractDateTimePartEnum datePart) =>
        datePart is
            ExtractDateTimePartEnum.Timezone or
            ExtractDateTimePartEnum.TimezoneHour or
            ExtractDateTimePartEnum.TimezoneMinute;
}
