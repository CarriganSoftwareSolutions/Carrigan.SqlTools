using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.DateTimeExpressions;

public sealed class DatePartTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public DatePartTests(BooksFixture fixture) => _fixture = fixture;

    public static IEnumerable<object[]> FunctionSpecificDateParts =>
        Enum.GetValues<DatePartDateTimePartEnum>().Select(static value => new object[] { value });

    private async Task<IEnumerable<IntegerValue>> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<Book> builder = new()
        {
            Selects = new SelectTags(new SelectTag(expression, "Value"))
        };
        SqlQuery query = _generator.Select(builder);
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        return await CommandsAsync.ExecuteReaderAsync<IntegerValue>(query, null, connection);
    }

    [Fact]
    public async Task SharedAndFunctionSpecificConstructors_Test()
    {
        DateTime value = new(2026, 9, 23, 6, 30, 15);
        IEnumerable<IntegerValue> shared = await ExecuteAsync(new DatePart(SharedDateTimePartEnum.Day, new Parameter(value)));
        IEnumerable<IntegerValue> specific = await ExecuteAsync(new DatePart(DatePartDateTimePartEnum.Month, new Parameter(value)));

        Assert.All(shared, record => Assert.Equal(23, record.Value));
        Assert.All(specific, record => Assert.Equal(9, record.Value));
    }

    [Theory]
    [MemberData(nameof(FunctionSpecificDateParts))]
    public async Task FunctionSpecificEnumValue_Test(DatePartDateTimePartEnum datePart)
    {
        DateTimeOffset value = new(2026, 9, 23, 6, 30, 15, TimeSpan.Zero);
        IntegerValue[] records = [.. await ExecuteAsync(new DatePart(datePart, new Parameter(value)))];

        Assert.NotEmpty(records);
    }
}
