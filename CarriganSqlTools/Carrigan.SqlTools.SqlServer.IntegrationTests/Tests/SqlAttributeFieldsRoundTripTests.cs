//Ignore Spelling: SqlServer, varchar, varbinary, datetime, ntext

using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlServer.IntegrationTests.SqlServerModels;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests;

public sealed class SqlAttributeFieldsRoundTripTests : IClassFixture<SqlAttributeFieldsFixture>
{
    private readonly SqlAttributeFieldsFixture _fixture;
    private readonly SqlGenerator<SqlAttributeFieldsModel> _generator;

    public SqlAttributeFieldsRoundTripTests(SqlAttributeFieldsFixture fixture)
    {
        _fixture = fixture;
        _generator = new();
    }

    [Fact]
    public async Task RoundTrip_InsertAutoId_Then_SelectById()
    {
        await _fixture.ResetAsync();

        SqlAttributeFieldsModel toInsert = new()
        {
            AsciiFixedCharValue = "ABCD",
            UnicodeVarCharValue = "attribute nvarchar",
            VarCharMaxValue = "attribute varchar max",
            NVarCharMaxValue = "attribute nvarchar max",
            BinaryValue = [0x01, 0x02, 0x03],
            VarBinaryValue = [0x04, 0x05, 0x06],
            VarBinaryMaxValue = [0x07, 0x08, 0x09],
            ImageValue = [0x0A, 0x0B, 0x0C],
            DateValue = new DateOnly(2025, 01, 02),
            DateTimeValue = new DateTime(2025, 01, 02, 03, 04, 05),
            DateTime2Value = new DateTime(2025, 01, 02, 03, 04, 05, 123),
            DateTimeOffsetValue = new DateTimeOffset(2025, 01, 02, 03, 04, 05, 123, new TimeSpan(-4, 0, 0)),
            TimeValue = new TimeOnly(13, 14, 15, 123),
            DecimalValue = 98765.43m,
            FloatValue = 12345.6789d,
            MoneyValue = 123.45m,
            SmallMoneyValue = 67.89m,
            TextValue = "attribute text",
            NTextValue = "attribute ntext"
        };

        SqlQuery insertQuery = _generator.InsertAutoId(toInsert);

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        object? insertedIdObj = await CommandsAsync.ExecuteScalarAsync(insertQuery, null, connection);
        Guid? insertedId = (Guid?)insertedIdObj;
        Assert.NotNull(insertedId);

        SqlAttributeFieldsModel keyEntity = new() { Id = insertedId.Value };
        SqlQuery selectById = _generator.SelectById(keyEntity);

        IEnumerable<SqlAttributeFieldsModel> rows = await CommandsAsync.ExecuteReaderAsync<SqlAttributeFieldsModel>(selectById, transaction: null, connection);
        SqlAttributeFieldsModel loaded = rows.First();

        Assert.Equal(insertedId, loaded.Id);
        Assert.Equal(toInsert.AsciiFixedCharValue, loaded.AsciiFixedCharValue);
        Assert.Equal(toInsert.UnicodeVarCharValue, loaded.UnicodeVarCharValue);
        Assert.Equal(toInsert.VarCharMaxValue, loaded.VarCharMaxValue);
        Assert.Equal(toInsert.NVarCharMaxValue, loaded.NVarCharMaxValue);
        Assert.Equal(toInsert.BinaryValue, loaded.BinaryValue);
        Assert.Equal(toInsert.VarBinaryValue, loaded.VarBinaryValue);
        Assert.Equal(toInsert.VarBinaryMaxValue, loaded.VarBinaryMaxValue);
        Assert.Equal(toInsert.ImageValue, loaded.ImageValue);
        Assert.Equal(toInsert.DateValue, loaded.DateValue);
        Assert.Equal(toInsert.DateTimeValue, loaded.DateTimeValue);
        Assert.Equal(toInsert.DateTime2Value, loaded.DateTime2Value);
        Assert.Equal(toInsert.DateTimeOffsetValue, loaded.DateTimeOffsetValue);
        Assert.Equal(toInsert.TimeValue, loaded.TimeValue);
        Assert.Equal(toInsert.DecimalValue, loaded.DecimalValue);
        Assert.Equal(toInsert.FloatValue, loaded.FloatValue);
        Assert.Equal(toInsert.MoneyValue, loaded.MoneyValue);
        Assert.Equal(toInsert.SmallMoneyValue, loaded.SmallMoneyValue);
        Assert.Equal(toInsert.TextValue, loaded.TextValue);
        Assert.Equal(toInsert.NTextValue, loaded.NTextValue);
    }
}
