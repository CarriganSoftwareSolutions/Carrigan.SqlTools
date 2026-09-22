using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests.DateTimeExpressions;

public sealed class CurrentTimestampTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> BookSqlGenerator = new();

    public CurrentTimestampTests(BooksFixture fixture) =>
        _fixture = fixture;

    private async Task<(IEnumerable<DateTimeValue> Records, DateTime Before, DateTime After)> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<Book> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag(expression, nameof(DateTimeValue.Value))
            )
        };

        SqlQuery query = BookSqlGenerator.Select(selectBuilder);

        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
        await connection.OpenAsync();

        DateTime before = await GetDatabaseDateTimeAsync(connection);
        IEnumerable<DateTimeValue> records = await CommandsAsync.ExecuteReaderAsync<DateTimeValue>(query, null, connection);
        DateTime after = await GetDatabaseDateTimeAsync(connection);

        return (records, before, after);
    }

    private static async Task<DateTime> GetDatabaseDateTimeAsync(NpgsqlConnection connection)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = "SELECT CURRENT_TIMESTAMP";
        return (DateTime)(await command.ExecuteScalarAsync() ?? throw new InvalidOperationException());
    }

    private static void AssertDateTimes(IEnumerable<DateTimeValue> records, DateTime before, DateTime after)
    {
        DateTimeValue[] actualRecords = [.. records];

        Assert.NotEmpty(actualRecords);
        Assert.All(actualRecords, record => Assert.InRange(record.Value, before, after));
    }

    [Fact]
    public async Task CurrentTimestamp_Test()
    {
        (IEnumerable<DateTimeValue> records, DateTime before, DateTime after) = await ExecuteAsync(new CurrentTimeStamp());

        AssertDateTimes(records, before, after);
    }
}
