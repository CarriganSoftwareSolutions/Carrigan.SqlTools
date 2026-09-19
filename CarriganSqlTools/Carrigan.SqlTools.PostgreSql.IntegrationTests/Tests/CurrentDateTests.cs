using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests;

public sealed class CurrentDateTests : IClassFixture<BooksFixture>
{
    private readonly BooksFixture _fixture;
    private readonly SqlGenerator<Book> BookSqlGenerator = new();

    public CurrentDateTests(BooksFixture fixture) =>
        _fixture = fixture;

    private async Task<(IEnumerable<DateOnlyValue> Records, DateOnly Before, DateOnly After)> ExecuteAsync(SqlExpression expression)
    {
        SelectBuilder<Book> selectBuilder = new()
        {
            Selects = new SelectTags
            (
                new SelectTag(expression, nameof(DateOnlyValue.Value))
            )
        };

        SqlQuery query = BookSqlGenerator.Select(selectBuilder);

        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
        await connection.OpenAsync();

        DateOnly before = await GetDatabaseDateAsync(connection);
        IEnumerable<DateOnlyValue> records = await CommandsAsync.ExecuteReaderAsync<DateOnlyValue>(query, null, connection);
        DateOnly after = await GetDatabaseDateAsync(connection);

        return (records, before, after);
    }

    private static async Task<DateOnly> GetDatabaseDateAsync(NpgsqlConnection connection)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = "SELECT CURRENT_DATE";
        return (DateOnly)(await command.ExecuteScalarAsync() ?? throw new InvalidOperationException());
    }

    private static void AssertDates(IEnumerable<DateOnlyValue> records, DateOnly before, DateOnly after)
    {
        DateOnlyValue[] actualRecords = [.. records];

        Assert.NotEmpty(actualRecords);
        Assert.All(actualRecords, record => Assert.InRange(record.Value, before, after));
    }

    [Fact]
    public async Task CurrentDate_Test()
    {
        (IEnumerable<DateOnlyValue> records, DateOnly before, DateOnly after) = await ExecuteAsync(new CurrentDate());

        AssertDates(records, before, after);
    }
}
