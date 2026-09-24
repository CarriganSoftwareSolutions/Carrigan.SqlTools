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

public sealed class MakeIntervalTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public MakeIntervalTests(BooksFixture fixture) => _fixture = fixture;

    public static IEnumerable<object[]> SharedDateParts =>
        Enum.GetValues<SharedDateTimePartEnum>().Select(static value => new object[] { value });

    public static IEnumerable<object[]> FunctionSpecificDateParts =>
        Enum.GetValues<MakeIntervalDateTimePartEnum>().Select(static value => new object[] { value });

    private static DateTime Value => new(2026, 1, 15, 6, 30, 15);

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
            SharedDateTimePartEnum.Year => Value.AddYears(1),
            SharedDateTimePartEnum.Month => Value.AddMonths(1),
            SharedDateTimePartEnum.Week => Value.AddDays(7),
            SharedDateTimePartEnum.Day => Value.AddDays(1),
            SharedDateTimePartEnum.Hour => Value.AddHours(1),
            SharedDateTimePartEnum.Minute => Value.AddMinutes(1),
            SharedDateTimePartEnum.Second => Value.AddSeconds(1),
            _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
        };

        Add expression = new(new Parameter(Value), new MakeInterval(datePart, new Parameter(1)));
        DateTimeValue[] records = [.. await ExecuteAsync(expression)];

        Assert.NotEmpty(records);
        Assert.All(records, record => Assert.Equal(expected, record.Value));
    }

    [Theory]
    [MemberData(nameof(FunctionSpecificDateParts))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1042:The member referenced by the MemberData attribute returns untyped data rows", Justification = "<Pending>")]
    public async Task FunctionSpecificEnumValue_Test(MakeIntervalDateTimePartEnum datePart)
    {
        DateTime expected = datePart switch
        {
            MakeIntervalDateTimePartEnum.Year => Value.AddYears(1),
            MakeIntervalDateTimePartEnum.Month => Value.AddMonths(1),
            MakeIntervalDateTimePartEnum.Week => Value.AddDays(7),
            MakeIntervalDateTimePartEnum.Day => Value.AddDays(1),
            MakeIntervalDateTimePartEnum.Hour => Value.AddHours(1),
            MakeIntervalDateTimePartEnum.Minute => Value.AddMinutes(1),
            MakeIntervalDateTimePartEnum.Second => Value.AddSeconds(1),
            _ => throw new ArgumentOutOfRangeException(nameof(datePart), datePart, null)
        };

        Add expression = new(new Parameter(Value), new MakeInterval(datePart, new Parameter(1)));
        DateTimeValue[] records = [.. await ExecuteAsync(expression)];

        Assert.NotEmpty(records);
        Assert.All(records, record => Assert.Equal(expected, record.Value));
    }
}
