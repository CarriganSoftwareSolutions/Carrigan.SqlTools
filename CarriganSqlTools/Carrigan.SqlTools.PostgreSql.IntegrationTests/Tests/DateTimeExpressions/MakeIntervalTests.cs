using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.DateTimeExpressions;

public sealed class MakeIntervalTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> _generator = new();

    public MakeIntervalTests(BooksFixture fixture) => _fixture = fixture;

    public static IEnumerable<object[]> FunctionSpecificDateParts =>
        Enum.GetValues<MakeIntervalDateTimePartEnum>().Select(static value => new object[] { value });

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

    [Fact]
    public async Task SharedAndFunctionSpecificConstructors_Test()
    {
        DateTime value = new(2026, 9, 23, 6, 30, 15);
        IEnumerable<DateTimeValue> shared = await ExecuteAsync
        (
            new Add(new Parameter(value), new MakeInterval(SharedDateTimePartEnum.Day, new Parameter(2)))
        );
        IEnumerable<DateTimeValue> specific = await ExecuteAsync
        (
            new Add(new Parameter(value), new MakeInterval(MakeIntervalDateTimePartEnum.Month, new Parameter(1)))
        );

        Assert.All(shared, record => Assert.Equal(value.AddDays(2), record.Value));
        Assert.All(specific, record => Assert.Equal(value.AddMonths(1), record.Value));
    }

    [Theory]
    [MemberData(nameof(FunctionSpecificDateParts))]
    public async Task FunctionSpecificEnumValue_Test(MakeIntervalDateTimePartEnum datePart)
    {
        DateTime value = new(2026, 9, 23, 6, 30, 15);
        Add expression = new(new Parameter(value), new MakeInterval(datePart, new Parameter(1)));
        DateTimeValue[] records = [.. await ExecuteAsync(expression)];

        Assert.NotEmpty(records);
    }
}
