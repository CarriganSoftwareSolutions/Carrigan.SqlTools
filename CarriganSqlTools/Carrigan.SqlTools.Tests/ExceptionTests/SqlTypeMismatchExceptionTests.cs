using System.Data;
using System.Reflection;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.ExceptionTests;

public sealed class SqlTypeMismatchExceptionTests
{
    #region Test Model

    private sealed class TestModel
    {
        public char CharacterValue { get; set; }

        public string StringValue { get; set; } = string.Empty;

        public int Int32Value { get; set; }

        public double DoubleValue { get; set; }

        public double? NullableDoubleValue { get; set; }

        public DateTime DateTimeValue { get; set; }
    }

    private static PropertyInfo GetPropertyInfo(string propertyName)
    {
        PropertyInfo? propertyInfo = typeof(TestModel).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

        Assert.NotNull(propertyInfo);

        return propertyInfo!;
    }

    #endregion


    #region Property + Attribute Tests

    [Fact]
    public void Validate_PropertyAndAttribute_Compatible()
    {
        PropertyInfo propertyInfo = GetPropertyInfo(nameof(TestModel.CharacterValue));
        SqlCharAttribute sqlCharAttribute = new(EncodingEnum.Ascii, StorageTypeEnum.Fixed);

        SqlTypeMismatchException? exception = SqlTypeMismatchException.Validate(propertyInfo, sqlCharAttribute);

        Assert.Null(exception);
    }

    [Fact]
    public void Validate_PropertyAndAttribute_CompatibleNullable()
    {
        PropertyInfo propertyInfo = GetPropertyInfo(nameof(TestModel.NullableDoubleValue));
        SqlFloatAttribute sqlFloatAttribute = new();

        SqlTypeMismatchException? exception = SqlTypeMismatchException.Validate(propertyInfo, sqlFloatAttribute);

        Assert.Null(exception);
    }

    [Fact]
    public void Validate_PropertyAndAttribute_Incompatible_Exception()
    {
        PropertyInfo propertyInfo = GetPropertyInfo(nameof(TestModel.Int32Value));
        SqlCharAttribute sqlCharAttribute = new(EncodingEnum.Ascii, StorageTypeEnum.Fixed);

        SqlTypeMismatchException? exception = SqlTypeMismatchException.Validate(propertyInfo, sqlCharAttribute);

        Assert.NotNull(exception);
        Assert.IsType<SqlTypeMismatchException>(exception);
        Assert.Contains(nameof(TestModel.Int32Value), exception!.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Validate_PropertyAndAttribute_NullAttribute()
    {
        PropertyInfo propertyInfo = GetPropertyInfo(nameof(TestModel.StringValue));

        SqlTypeMismatchException? exception = SqlTypeMismatchException.Validate(propertyInfo, null);

        Assert.Null(exception);
    }

    #endregion


    #region Value + SqlDbType Tests

    [Fact]
    public void Validate_ValueAndSqlDbType_Compatible()
    {
        string value = "Test";
        SqlDbType sqlDbType = SqlDbType.NVarChar;

        SqlTypeMismatchException? exception = SqlTypeMismatchException.Validate(value, sqlDbType);

        Assert.Null(exception);
    }

    [Fact]
    public void Validate_ValueAndSqlDbType_CompatibleNumeric()
    {
        double value = 3.14159;
        SqlDbType sqlDbType = SqlDbType.Float;

        SqlTypeMismatchException? exception = SqlTypeMismatchException.Validate(value, sqlDbType);

        Assert.Null(exception);
    }

    [Fact]
    public void Validate_ValueAndSqlDbType_Incompatible_Exception()
    {
        string value = "Test";
        SqlDbType sqlDbType = SqlDbType.Int;

        SqlTypeMismatchException? exception = SqlTypeMismatchException.Validate(value, sqlDbType);

        Assert.NotNull(exception);
        Assert.IsType<SqlTypeMismatchException>(exception);
        Assert.Contains(nameof(String), exception!.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Validate_ValueAndSqlDbType_Variant()
    {
        string value = "Test";
        SqlDbType sqlDbType = SqlDbType.Variant;

        SqlTypeMismatchException? exception = SqlTypeMismatchException.Validate(value, sqlDbType);

        Assert.Null(exception);
    }

    [Fact]
    public void Validate_ValueAndSqlDbType_NullValue()
    {
        object? value = null;
        SqlDbType sqlDbType = SqlDbType.NVarChar;

        SqlTypeMismatchException? exception = SqlTypeMismatchException.Validate(value!, sqlDbType);

        Assert.Null(exception);
    }

    #endregion


    #region Exhaustive Cross-Matrix Tests

    private static readonly SqlDbType[] AllSqlDbTypes =
        (SqlDbType[])Enum.GetValues<SqlDbType>();

    private static readonly IReadOnlyDictionary<Type, SqlDbType[]> ExpectedAllowedSqlDbTypeMappings =
        new Dictionary<Type, SqlDbType[]>
        (
            [
                new KeyValuePair<Type, SqlDbType[]>(typeof(Guid),
                [
                    SqlDbType.UniqueIdentifier
                ]),

                new KeyValuePair<Type, SqlDbType[]>(typeof(char),
                [
                    SqlDbType.Char, SqlDbType.NChar, SqlDbType.VarChar,
                    SqlDbType.NVarChar, SqlDbType.Text, SqlDbType.NText
                ]),
                new KeyValuePair<Type, SqlDbType[]>(typeof(string),
                [
                    SqlDbType.Char, SqlDbType.NChar, SqlDbType.VarChar,
                    SqlDbType.NVarChar, SqlDbType.Text, SqlDbType.NText
                ]),
                new KeyValuePair<Type, SqlDbType[]>(typeof(byte[]),
                [
                    SqlDbType.Binary, SqlDbType.VarBinary, SqlDbType.Image
                ]),

                new KeyValuePair<Type, SqlDbType[]>(typeof(bool),
                [
                    SqlDbType.Bit
                ]),

                new KeyValuePair<Type, SqlDbType[]>(typeof(byte),
                [
                    SqlDbType.TinyInt
                ]),
                new KeyValuePair<Type, SqlDbType[]>(typeof(sbyte),
                [
                    SqlDbType.SmallInt
                ]),
                new KeyValuePair<Type, SqlDbType[]>(typeof(short),
                [
                    SqlDbType.SmallInt
                ]),
                new KeyValuePair<Type, SqlDbType[]>(typeof(int),
                [
                    SqlDbType.Int
                ]),
                new KeyValuePair<Type, SqlDbType[]>(typeof(long),
                [
                    SqlDbType.BigInt
                ]),

                new KeyValuePair<Type, SqlDbType[]>(typeof(float),
                [
                    SqlDbType.Real, SqlDbType.Float, SqlDbType.Decimal,
                    SqlDbType.Money, SqlDbType.SmallMoney
                ]),
                new KeyValuePair<Type, SqlDbType[]>(typeof(double),
                [
                    SqlDbType.Real, SqlDbType.Float, SqlDbType.Decimal,
                    SqlDbType.Money, SqlDbType.SmallMoney
                ]),
                new KeyValuePair<Type, SqlDbType[]>(typeof(decimal),
                [
                    SqlDbType.Real, SqlDbType.Float, SqlDbType.Decimal,
                    SqlDbType.Money, SqlDbType.SmallMoney
                ]),

                new KeyValuePair<Type, SqlDbType[]>(typeof(DateTime),
                [
                    SqlDbType.SmallDateTime, SqlDbType.DateTime, SqlDbType.DateTime2
                ]),
                new KeyValuePair<Type, SqlDbType[]>(typeof(DateOnly),
                [
                    SqlDbType.SmallDateTime, SqlDbType.DateTime, SqlDbType.DateTime2
                ]),
                new KeyValuePair<Type, SqlDbType[]>(typeof(TimeOnly),
                [
                    SqlDbType.SmallDateTime, SqlDbType.DateTime, SqlDbType.DateTime2
                ]),
                new KeyValuePair<Type, SqlDbType[]>(typeof(DateTimeOffset),
                [
                    SqlDbType.SmallDateTime, SqlDbType.DateTime, SqlDbType.DateTime2, SqlDbType.Time
                ])
            ]
        );

    [Fact]
    public void Validate_ValueAndSqlDbType_AllTypeCombinations()
    {
        foreach (KeyValuePair<Type, SqlDbType[]> mapping in ExpectedAllowedSqlDbTypeMappings)
        {
            Type runtimeType = mapping.Key;
            SqlDbType[] allowedSqlDbTypes = mapping.Value;

            foreach (SqlDbType sqlDbType in AllSqlDbTypes)
            {
                bool isAllowed =
                    sqlDbType == SqlDbType.Variant ||
                    Array.IndexOf(allowedSqlDbTypes, sqlDbType) >= 0;

                object value = CreateSampleValue(runtimeType);

                SqlTypeMismatchException? exception =
                    SqlTypeMismatchException.Validate(value, sqlDbType);

                if (isAllowed)
                    Assert.Null(exception);
                else
                    Assert.NotNull(exception);
            }
        }
    }

    private static object CreateSampleValue(Type runtimeType)
    {
        if (runtimeType == typeof(Guid)) return Guid.NewGuid();
        if (runtimeType == typeof(char)) return 'A';
        if (runtimeType == typeof(string)) return "Test";
        if (runtimeType == typeof(byte[])) return new byte[] { 1, 2, 3 };
        if (runtimeType == typeof(bool)) return true;
        if (runtimeType == typeof(byte)) return (byte)42;
        if (runtimeType == typeof(sbyte)) return (sbyte)-42;
        if (runtimeType == typeof(short)) return (short)1234;
        if (runtimeType == typeof(int)) return 1234;
        if (runtimeType == typeof(long)) return 1234L;
        if (runtimeType == typeof(float)) return 1.23F;
        if (runtimeType == typeof(double)) return 1.23D;
        if (runtimeType == typeof(decimal)) return 1.23M;
        if (runtimeType == typeof(DateTime)) return DateTime.UtcNow;

        if (runtimeType == typeof(DateOnly))
        {
            DateTime now = DateTime.UtcNow;
            return new DateOnly(now.Year, now.Month, now.Day);
        }

        if (runtimeType == typeof(TimeOnly))
        {
            DateTime now = DateTime.UtcNow;
            return new TimeOnly(now.Hour, now.Minute, now.Second);
        }

        if (runtimeType == typeof(DateTimeOffset)) return DateTimeOffset.UtcNow;

        throw new NotSupportedException($"Unsupported CLR type: {runtimeType.FullName}");
    }

    #endregion
}
