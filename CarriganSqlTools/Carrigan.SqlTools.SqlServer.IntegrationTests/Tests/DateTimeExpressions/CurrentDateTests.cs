using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests.DateTimeExpressions;

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

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        await connection.OpenAsync();

        await RequireCurrentDateSupportAsync(connection);

        DateOnly before = await GetDatabaseDateAsync(connection);
        IEnumerable<DateOnlyValue> records = await CommandsAsync.ExecuteReaderAsync<DateOnlyValue>(query, null, connection);
        DateOnly after = await GetDatabaseDateAsync(connection);

        return (records, before, after);
    }

    private static async Task RequireCurrentDateSupportAsync(SqlConnection connection)
    {
        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = "SELECT CONVERT(int, SERVERPROPERTY('ProductMajorVersion'))";
        int productMajorVersion = Convert.ToInt32(await command.ExecuteScalarAsync());

        if (productMajorVersion < 17)
            Assert.Skip("CURRENT_DATE requires SQL Server 2025 (17.x) or later.");
    }

    private static async Task<DateOnly> GetDatabaseDateAsync(SqlConnection connection)
    {
        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = "SELECT CAST(GETDATE() AS date)";
        DateTime value = (DateTime)(await command.ExecuteScalarAsync() ?? throw new InvalidOperationException());
        return DateOnly.FromDateTime(value);
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
