//Ignore Spelling: PostgreSql, varchar

using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.PostgreSqlModels;
using Carrigan.SqlTools.SqlGenerators;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests;

public sealed class AttributeFieldsRoundTripTests : IClassFixture<AttributeFieldsFixture>
{
    private readonly AttributeFieldsFixture _fixture;
    private readonly SqlGenerator<AttributeFieldsModel> _generator;

    public AttributeFieldsRoundTripTests(AttributeFieldsFixture fixture)
    {
        _fixture = fixture;
        _generator = new();
    }

    [Fact]
    public async Task RoundTrip_InsertAutoId_Then_SelectById()
    {
        await _fixture.ResetAsync();

        AttributeFieldsModel toInsert = new()
        {
            FixedCharValue = "ABCD",
            VarCharValue = "attribute varchar",
            FloatValue = 123.25f,
            MoneyValue = 123.45m,
            NumericValue = 98765.43m,
            DateValue = new DateOnly(2025, 01, 02),
            TimeValue = new TimeOnly(13, 14, 15, 123),
            TimestampValue = new DateTime(2025, 01, 02, 03, 04, 05, 123, DateTimeKind.Utc),
            TimestampUtcValue = new DateTimeOffset(2025, 01, 02, 03, 04, 05, 123, new TimeSpan(-4, 0, 0)),
            NumericArrayValue = [null, -12345.67m, 12345.67m],
            VarCharArrayValue = [null, string.Empty, "array varchar"]
        };

        SqlQuery insertQuery = _generator.InsertAutoId(toInsert);

        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
        object? insertedIdObj = await CommandsAsync.ExecuteScalarAsync(insertQuery, null, connection);
        Guid? insertedId = (Guid?)insertedIdObj;
        Assert.NotNull(insertedId);

        AttributeFieldsModel keyEntity = new() { Id = insertedId.Value };
        SqlQuery selectById = _generator.SelectById(keyEntity);

        IEnumerable<AttributeFieldsModel> rows = await CommandsAsync.ExecuteReaderAsync<AttributeFieldsModel>(selectById, transaction: null, connection);
        AttributeFieldsModel loaded = rows.First();

        Assert.Equal(insertedId, loaded.Id);
        Assert.Equal(toInsert.FixedCharValue, loaded.FixedCharValue);
        Assert.Equal(toInsert.VarCharValue, loaded.VarCharValue);
        Assert.Equal(toInsert.FloatValue, loaded.FloatValue);
        Assert.Equal(toInsert.MoneyValue, loaded.MoneyValue);
        Assert.Equal(toInsert.NumericValue, loaded.NumericValue);
        Assert.Equal(toInsert.DateValue, loaded.DateValue);
        Assert.Equal(toInsert.TimeValue, loaded.TimeValue);
        Assert.Equal(DateTime.SpecifyKind(toInsert.TimestampValue, DateTimeKind.Unspecified), loaded.TimestampValue);
        Assert.Equal(toInsert.TimestampUtcValue.ToUniversalTime(), loaded.TimestampUtcValue);
        Assert.Equal(toInsert.NumericArrayValue, loaded.NumericArrayValue);
        Assert.Equal(toInsert.VarCharArrayValue, loaded.VarCharArrayValue);
    }
}
