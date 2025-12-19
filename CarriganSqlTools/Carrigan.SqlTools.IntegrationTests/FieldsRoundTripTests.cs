//Ignore Spelling: SqlTools, Localdb, Respawn, Respawner, Carrigan, SqlServer, DateOnly, TimeOnly, XDocument, XmlDocument, lorem ipsum

using Carrigan.SqlTools.IntegrationTests.Fixtures;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;

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

    [Theory]
    [MemberData(nameof(BasicTestData))]
    [MemberData(nameof(MinTestData))]
    [MemberData(nameof(MaxTestData))]
    public async Task RoundTrip_InsertAutoId_Then_SelectById(FieldsModel toInsert)
    {
        await _fixture.ResetAsync();

        // 1) Build InsertAutoId and execute scalar to get the new identity value
        SqlQuery insertQuery = _generator.InsertAutoId(toInsert);

        SqlConnection connection = new(_fixture.ConnectionString);
        object? insertedIdObj = await CommandsAsync.ExecuteScalarAsync(insertQuery, null, connection);
        Guid? insertedId = (Guid?)insertedIdObj;
        Assert.NotNull(insertedId);
        // 2) Build SelectById and read the row back
        FieldsModel keyEntity = new() { Id = insertedId.Value };
        SqlQuery selectById = _generator.SelectById(keyEntity);

        IEnumerable<FieldsModel> rows = await CommandsAsync.ExecuteReaderAsync<FieldsModel>(selectById, transaction: null, connection);

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
        Assert.Equal(toInsert.XDocumentValue?.ToString(), loaded.XDocumentValue?.ToString());
        Assert.Equal(toInsert.XmlDocumentValue?.OuterXml, loaded.XmlDocumentValue?.OuterXml);

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
        Assert.Null(loaded.XDocumentNullableValue);
        Assert.Null(loaded.XmlDocumentNullableValue);
    }

    public static TheoryData<FieldsModel> BasicTestData =>
        new
        (
            [
                new FieldsModel()
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
                    //TODO: Remove or replace TimeSpan code
                    //TimeSpanValue = new TimeSpan(0, 2, 3, 4, 567),
                    //TimeSpanNullableValue = null,
                    DateTimeOffsetValue = new DateTimeOffset(1969, 7, 20, 16, 17, 0, new TimeSpan(-4, 0, 0)),
                    DateTimeOffsetNullableValue = null,

                    BytesValue = [ 0x01, 0x02, 0x03 ],

                    XDocumentValue = null,
                    XmlDocumentValue = null,
                    XDocumentNullableValue = null,
                    XmlDocumentNullableValue = null
                }
            ]
        );

    public static TheoryData<FieldsModel> MinTestData =>
        new
        (
            [
                new FieldsModel()
                {
                    // PK omitted for InsertAutoId
                    GuidValue = Guid.Empty,
                    GuidNullableValue = null,

                    StringValue = string.Empty,
                    CharValue = char.MinValue,
                    CharNullableValue = null,

                    IntValue = int.MinValue,
                    IntNullableValue = null,
                    LongValue = long.MinValue,
                    LongNullableValue = null,
                    ShortValue = short.MinValue,
                    ShortNullableValue = null,
                    ByteValue = byte.MinValue,
                    ByteNullableValue = null,

                    BoolValue = true,
                    BoolNullableValue = null,

                    DecimalValue = -99999999999999.9999m, //TODO: these values where recommended, revisit
                    DecimalNullableValue = null,
                    DoubleValue = double.MinValue,
                    DoubleNullableValue = null,
                    FloatValue = float.MinValue,
                    FloatNullableValue = null,

                    DateOnlyValue = DateOnly.MinValue,
                    DateOnlyNullableValue = null,
                    TimeOnlyValue = TimeOnly.MinValue,
                    TimeOnlyNullableValue = null,
                    DateTimeValue = DateTime.MinValue,
                    DateTimeNullableValue = null,
                    //TODO: Remove or replace TimeSpan code
                    //TimeSpanValue = new TimeSpan(0, 2, 3, 4, 567),
                    //TimeSpanNullableValue = null,
                    DateTimeOffsetValue = DateTimeOffset.MinValue,
                    DateTimeOffsetNullableValue = null,

                    BytesValue = [],

                    XDocumentValue = null,
                    XmlDocumentValue = null,
                    XDocumentNullableValue = null,
                    XmlDocumentNullableValue = null
                }
            ]
        );

    public static TheoryData<FieldsModel> MaxTestData
    {
        get
        {
            FieldsModel fieldsModel = new ()
            {
                // PK omitted for InsertAutoId
                GuidValue = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"),
                GuidNullableValue = null,
                //             0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001
                //             0000000001111111111222222222233333333334444444444555555555566666666667777777777888888888899999999990 
                StringValue = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"
                // 1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111112
                // 0000000001111111111222222222233333333334444444444555555555566666666667777777777888888888899999999990 
                + "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890",

                CharValue = char.MaxValue,
                CharNullableValue = null,

                IntValue = int.MaxValue,
                IntNullableValue = null,
                LongValue = long.MaxValue,
                LongNullableValue = null,
                ShortValue = short.MaxValue,
                ShortNullableValue = null,
                ByteValue = byte.MaxValue,
                ByteNullableValue = null,

                BoolValue = true,
                BoolNullableValue = null,

                DecimalValue = 99999999999999.9999m, //TODO: these values where recommended, revisit
                DecimalNullableValue = null,
                DoubleValue = double.MaxValue,
                DoubleNullableValue = null,
                FloatValue = float.MaxValue,
                FloatNullableValue = null,

                DateOnlyValue = DateOnly.MaxValue,
                DateOnlyNullableValue = null,
                TimeOnlyValue = TimeOnly.MaxValue,
                TimeOnlyNullableValue = null,
                DateTimeValue = DateTime.MaxValue,
                DateTimeNullableValue = null,
                //TODO: Remove comments or restore
                //TimeSpanValue = new TimeSpan(0, 2, 3, 4, 567),
                //TimeSpanNullableValue = null,
                DateTimeOffsetValue = DateTimeOffset.MaxValue,
                DateTimeOffsetNullableValue = null,

                BytesValue = [],

                //Testing max is no really feasible for this value
                XDocumentValue = new
                (
                    new XElement
                    (
                        "root",
                        new XElement("value", "Hello World")
                    )
                ),

                //Testing max is no really feasible for this value
                XmlDocumentValue = new(),
                XDocumentNullableValue = null,
                XmlDocumentNullableValue = null
            };

            fieldsModel.XmlDocumentValue.LoadXml
            (
                "<root><value>Hello World</value></root>"
            );
            return new ([fieldsModel]);
        }
    }
}
