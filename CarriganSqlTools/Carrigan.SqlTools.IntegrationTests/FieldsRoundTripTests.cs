//Ignore Spelling: SqlTools, Localdb, Respawn, Respawner, Carrigan, SqlServer, DateOnly, TimeOnly, XDocument, XmlDocument, lorem ipsum

using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.IntegrationTests.Fixtures;
using Carrigan.SqlTools.IntegrationTests.Models;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.IntegrationTests;

public sealed class FieldsRoundTripTests : IClassFixture<FieldsFixture>
{
    private readonly FieldsFixture _fixture;
    private readonly SqlGenerator<FieldsModel> _generator;

    public FieldsRoundTripTests(FieldsFixture fixture)
    {
        _fixture = fixture;
        _generator = new();
    }

    [Fact]
    public async Task RoundTrip_InsertAutoId_Then_SelectById()
    {
        await _fixture.ResetAsync();

        // Prepare an entity with values for NOT NULL columns (Id is auto)
        FieldsModel toInsert = new()
        {
            // PK omitted for InsertAutoId
            GuidValue = Guid.CreateVersion7(),
            GuidNullableValue = null,

            StringValue = "lorem ipsum",
            CharValue = 'Z',
            CharNullableValue = null,

            IntValue = 123,
            IntNullableValue = null,
            LongValue = 9876543210L,
            LongNullableValue = null,
            ShortValue = 32123,
            ShortNullableValue = null,
            ByteValue = 200,
            ByteNullableValue = null,

            BoolValue = true,
            BoolNullableValue = null,

            DecimalValue = 123.4567m,
            DecimalNullableValue = null,
            DoubleValue = 3.1415926535d,
            DoubleNullableValue = null,
            FloatValue = 2.71828f,
            FloatNullableValue = null,

            DateOnlyValue = new DateOnly(2025, 01, 02),
            DateOnlyNullableValue = null,
            TimeOnlyValue = new TimeOnly(13, 14, 15, 123),
            TimeOnlyNullableValue = null,
            DateTimeValue = new DateTime(2025, 01, 02, 03, 04, 05, 0, DateTimeKind.Utc), //Note: not testing milliseconds because of precision errors
            DateTimeNullableValue = null,
            //TimeSpanValue = new TimeSpan(0, 2, 3, 4, 567),
            //TimeSpanNullableValue = null,
            DateTimeOffsetValue = new DateTimeOffset(1969, 7, 20, 16, 17, 0, new TimeSpan(-4, 0, 0)),
            DateTimeOffsetNullableValue = null,

            BytesValue = [ 0x01, 0x02, 0x03 ],

            XDocumentValue = null,   // set to a value if your mapper handles XDocument
            XmlDocumentValue = null  // set to a value if your mapper handles XmlDocument
        };

        // 1) Build InsertAutoId and execute scalar to get the new identity value
        SqlQuery insertQuery = _generator.InsertAutoId(toInsert);

        SqlConnection connection = new(_fixture.ConnectionString);
        object? insertedIdObj = await CommandsAsync.ExecuteScalarAsync(insertQuery, null, connection);
        Guid? insertedId = (Guid?)insertedIdObj;
        Assert.NotNull(insertedId);
        // 2) Build SelectById and read the row back
        FieldsModel keyEntity = new() { Id = insertedId.Value };
        SqlQuery selectById = _generator.SelectById(keyEntity);

        IEnumerable<FieldsModel> rows =
            await CommandsAsync.ExecuteReaderAsync<FieldsModel>(selectById, transaction: null, connection);

        FieldsModel loaded = rows.First();

        // 3) Assert round-trip values
        Assert.Equal(insertedId, loaded.Id);
        Assert.Equal(toInsert.GuidValue, loaded.GuidValue);
        Assert.Equal(toInsert.StringValue, loaded.StringValue);
        Assert.Equal(toInsert.CharValue, loaded.CharValue);
        Assert.Equal(toInsert.IntValue, loaded.IntValue);
        Assert.Equal(toInsert.LongValue, loaded.LongValue);
        Assert.Equal(toInsert.ShortValue, loaded.ShortValue);
        Assert.Equal(toInsert.ByteValue, loaded.ByteValue);
        Assert.Equal(toInsert.BoolValue, loaded.BoolValue);
        Assert.Equal(toInsert.DecimalValue, loaded.DecimalValue);
        Assert.Equal(toInsert.DoubleValue, loaded.DoubleValue, 6);
        Assert.Equal(toInsert.FloatValue, loaded.FloatValue, 3);
        Assert.Equal(toInsert.DateOnlyValue, loaded.DateOnlyValue);
        Assert.Equal(toInsert.TimeOnlyValue, loaded.TimeOnlyValue);
        Assert.Equal(toInsert.DateTimeValue, loaded.DateTimeValue);
        //Assert.Equal(toInsert.TimeSpanValue, loaded.TimeSpanValue);
        Assert.Equal(toInsert.DateTimeOffsetValue, loaded.DateTimeOffsetValue);
        Assert.NotNull(loaded.BytesValue);
        Assert.Equal(toInsert.BytesValue!.Length, loaded.BytesValue!.Length);

        // Nullable value assertions
        Assert.Null(loaded.GuidNullableValue);
        Assert.Null(loaded.CharNullableValue);
        Assert.Null(loaded.IntNullableValue);
        Assert.Null(loaded.LongNullableValue);
        Assert.Null(loaded.ShortNullableValue);
        Assert.Null(loaded.ByteNullableValue);
        Assert.Null(loaded.BoolNullableValue);
        Assert.Null(loaded.DecimalNullableValue);
        Assert.Null(loaded.DoubleNullableValue);
        Assert.Null(loaded.FloatNullableValue);
        Assert.Null(loaded.DateOnlyNullableValue);
        Assert.Null(loaded.TimeOnlyNullableValue);
        Assert.Null(loaded.DateTimeNullableValue);
        //Assert.Null(loaded.TimeSpanNullableValue);
        Assert.Null(loaded.DecimalNullableValue);
        Assert.Null(loaded.XDocumentValue);
        Assert.Null(loaded.XmlDocumentValue);
    }
}
