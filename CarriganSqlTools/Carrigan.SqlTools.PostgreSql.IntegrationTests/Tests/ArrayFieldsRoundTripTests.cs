//Ignore Spelling: PostgreSql, bytea, XDocument, XmlDocument

using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Npgsql;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests;

public sealed class ArrayFieldsRoundTripTests : IClassFixture<ArrayFieldsFixture>
{
    private readonly ArrayFieldsFixture _fixture;
    private readonly SqlGenerator<ArrayFieldsModel> _generator;

    public ArrayFieldsRoundTripTests(ArrayFieldsFixture fixture)
    {
        _fixture = fixture;
        _generator = new();
    }

    [Theory]
    [MemberData(nameof(ArrayFieldsDataSet.EmptyArrays), MemberType = typeof(ArrayFieldsDataSet))]
    [MemberData(nameof(ArrayFieldsDataSet.NullMinMaxArrays), MemberType = typeof(ArrayFieldsDataSet))]
    public async Task RoundTrip_InsertAutoId_Then_SelectById(ArrayFieldsModel toInsert)
    {
        await _fixture.ResetAsync();

        SqlQuery insertQuery = _generator.InsertAutoId(toInsert);

        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);
        object? insertedIdObj = await CommandsAsync.ExecuteScalarAsync(insertQuery, null, connection);
        Guid? insertedId = (Guid?)insertedIdObj;
        Assert.NotNull(insertedId);

        ArrayFieldsModel keyEntity = new() { Id = insertedId.Value };
        SqlQuery selectById = _generator.SelectById(keyEntity);

        IEnumerable<ArrayFieldsModel> rows = await CommandsAsync.ExecuteReaderAsync<ArrayFieldsModel>(selectById, transaction: null, connection);
        ArrayFieldsModel loaded = rows.First();

        Assert.Equal(insertedId, loaded.Id);
        AssertArraysEqual(toInsert.GuidArrayValue, loaded.GuidArrayValue);
        AssertArraysEqual(toInsert.GuidNullableArrayValue, loaded.GuidNullableArrayValue);
        AssertArraysEqual(toInsert.StringArrayValue, loaded.StringArrayValue);
        AssertArraysEqual(toInsert.CharArrayValue, loaded.CharArrayValue);
        AssertArraysEqual(toInsert.CharNullableArrayValue, loaded.CharNullableArrayValue);
        AssertByteArraysEqual(toInsert.BytesArrayValue, loaded.BytesArrayValue);
        AssertArraysEqual(toInsert.BoolArrayValue, loaded.BoolArrayValue);
        AssertArraysEqual(toInsert.BoolNullableArrayValue, loaded.BoolNullableArrayValue);
        AssertArraysEqual(toInsert.ByteNullableArrayValue, loaded.ByteNullableArrayValue);
        AssertArraysEqual(toInsert.SByteArrayValue, loaded.SByteArrayValue);
        AssertArraysEqual(toInsert.SByteNullableArrayValue, loaded.SByteNullableArrayValue);
        AssertArraysEqual(toInsert.ShortArrayValue, loaded.ShortArrayValue);
        AssertArraysEqual(toInsert.ShortNullableArrayValue, loaded.ShortNullableArrayValue);
        AssertArraysEqual(toInsert.UShortArrayValue, loaded.UShortArrayValue);
        AssertArraysEqual(toInsert.UShortNullableArrayValue, loaded.UShortNullableArrayValue);
        AssertArraysEqual(toInsert.IntArrayValue, loaded.IntArrayValue);
        AssertArraysEqual(toInsert.IntNullableArrayValue, loaded.IntNullableArrayValue);
        AssertArraysEqual(toInsert.UIntArrayValue, loaded.UIntArrayValue);
        AssertArraysEqual(toInsert.UIntNullableArrayValue, loaded.UIntNullableArrayValue);
        AssertArraysEqual(toInsert.LongArrayValue, loaded.LongArrayValue);
        AssertArraysEqual(toInsert.LongNullableArrayValue, loaded.LongNullableArrayValue);
        AssertArraysEqual(toInsert.ULongArrayValue, loaded.ULongArrayValue);
        AssertArraysEqual(toInsert.ULongNullableArrayValue, loaded.ULongNullableArrayValue);
        AssertArraysEqual(toInsert.FloatArrayValue, loaded.FloatArrayValue);
        AssertArraysEqual(toInsert.FloatNullableArrayValue, loaded.FloatNullableArrayValue);
        AssertArraysEqual(toInsert.DoubleArrayValue, loaded.DoubleArrayValue);
        AssertArraysEqual(toInsert.DoubleNullableArrayValue, loaded.DoubleNullableArrayValue);
        AssertArraysEqual(toInsert.DecimalArrayValue, loaded.DecimalArrayValue);
        AssertArraysEqual(toInsert.DecimalNullableArrayValue, loaded.DecimalNullableArrayValue);
        AssertArraysEqual(toInsert.DateOnlyArrayValue, loaded.DateOnlyArrayValue);
        AssertArraysEqual(toInsert.DateOnlyNullableArrayValue, loaded.DateOnlyNullableArrayValue);
        AssertArraysEqual(toInsert.TimeOnlyArrayValue, loaded.TimeOnlyArrayValue);
        AssertArraysEqual(toInsert.TimeOnlyNullableArrayValue, loaded.TimeOnlyNullableArrayValue);
        AssertArraysEqual(toInsert.DateTimeArrayValue?.Select(NormalizeDateTime).ToArray(), loaded.DateTimeArrayValue);
        AssertArraysEqual(toInsert.DateTimeNullableArrayValue?.Select(NormalizeDateTime).ToArray(), loaded.DateTimeNullableArrayValue);
        AssertArraysEqual(toInsert.TimeSpanArrayValue, loaded.TimeSpanArrayValue);
        AssertArraysEqual(toInsert.TimeSpanNullableArrayValue, loaded.TimeSpanNullableArrayValue);
        AssertArraysEqual(toInsert.DateTimeOffsetArrayValue?.Select(NormalizeDateTimeOffset).ToArray(), loaded.DateTimeOffsetArrayValue);
        AssertArraysEqual(toInsert.DateTimeOffsetNullableArrayValue?.Select(NormalizeDateTimeOffset).ToArray(), loaded.DateTimeOffsetNullableArrayValue);
        AssertXDocumentsEqual(toInsert.XDocumentArrayValue, loaded.XDocumentArrayValue);
        AssertXmlDocumentsEqual(toInsert.XmlDocumentArrayValue, loaded.XmlDocumentArrayValue);
        AssertArraysEqual(toInsert.ObjectArrayValue?.Select(value => value?.ToString()).ToArray(), loaded.ObjectArrayValue?.Select(value => value?.ToString()).ToArray());
    }

    private static DateTime NormalizeDateTime(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
    private static DateTime? NormalizeDateTime(DateTime? value) => value is null ? null : NormalizeDateTime(value.Value);
    private static DateTimeOffset NormalizeDateTimeOffset(DateTimeOffset value) => value.ToUniversalTime();
    private static DateTimeOffset? NormalizeDateTimeOffset(DateTimeOffset? value) => value?.ToUniversalTime();

    private static void AssertArraysEqual<T>(T[]? expected, T[]? actual)
    {
        if (expected is null)
        {
            Assert.Null(actual);
            return;
        }

        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }

    private static void AssertByteArraysEqual(byte[]?[]? expected, byte[]?[]? actual)
    {
        if (expected is null)
        {
            Assert.Null(actual);
            return;
        }

        Assert.NotNull(actual);
        Assert.Equal(expected.Length, actual.Length);

        for (int index = 0; index < expected.Length; index++)
            AssertArraysEqual(expected[index], actual[index]);
    }

    private static void AssertXDocumentsEqual(XDocument?[]? expected, XDocument?[]? actual)
    {
        if (expected is null)
        {
            Assert.Null(actual);
            return;
        }

        Assert.NotNull(actual);
        Assert.Equal(expected.Length, actual.Length);

        for (int index = 0; index < expected.Length; index++)
            Assert.Equal(expected[index]?.ToString(SaveOptions.DisableFormatting), actual[index]?.ToString(SaveOptions.DisableFormatting));
    }

    private static void AssertXmlDocumentsEqual(XmlDocument?[]? expected, XmlDocument?[]? actual)
    {
        if (expected is null)
        {
            Assert.Null(actual);
            return;
        }

        Assert.NotNull(actual);
        Assert.Equal(expected.Length, actual.Length);

        for (int index = 0; index < expected.Length; index++)
            Assert.Equal(expected[index]?.OuterXml, actual[index]?.OuterXml);
    }
}
