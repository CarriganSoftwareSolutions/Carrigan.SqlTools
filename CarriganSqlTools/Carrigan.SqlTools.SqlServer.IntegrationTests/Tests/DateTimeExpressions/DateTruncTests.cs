using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.DateTimeExpressions;

public sealed class DateTruncTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public DateTruncTests(BooksFixture fixture) => _fixture = fixture;

    public static IEnumerable<object[]> FunctionSpecificDateParts =>
        Enum.GetValues<DateTruncDateTimePartEnum>().Select(static value => new object[] { value });

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
        IEnumerable<DateTimeValue> shared = await ExecuteAsync(new DateTrunc(SharedDateTimePartEnum.Day, new Parameter(value)));
        IEnumerable<DateTimeValue> specific = await ExecuteAsync(new DateTrunc(DateTruncDateTimePartEnum.Month, new Parameter(value)));

        Assert.All(shared, record => Assert.Equal(new DateTime(2026, 9, 23), record.Value));
        Assert.All(specific, record => Assert.Equal(new DateTime(2026, 9, 1), record.Value));
    }

    [Theory]
    [MemberData(nameof(FunctionSpecificDateParts))]
    public async Task FunctionSpecificEnumValue_Test(DateTruncDateTimePartEnum datePart)
    {
        DateTime value = new DateTime(2026, 9, 23, 6, 30, 15, 123).AddTicks(4567);
        DateTimeValue[] records = [.. await ExecuteAsync(new DateTrunc(datePart, new Parameter(value)))];

        Assert.NotEmpty(records);
    }
}
