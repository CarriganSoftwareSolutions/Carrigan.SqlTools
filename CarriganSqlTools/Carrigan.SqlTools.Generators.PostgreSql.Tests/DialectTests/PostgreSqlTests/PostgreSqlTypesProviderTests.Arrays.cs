using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;


namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Theory]
    [InlineData("Uuid", "UUID[] NOT NULL")]
    [InlineData("Char", "CHAR(10)[] NOT NULL")]
    [InlineData("VarChar", "VARCHAR(50)[] NOT NULL")]
    [InlineData("Text", "TEXT[] NOT NULL")]
    [InlineData("Bytea", "BYTEA[] NOT NULL")]
    [InlineData("Boolean", "BOOLEAN[] NOT NULL")]
    [InlineData("SmallInt", "SMALLINT[] NOT NULL")]
    [InlineData("Integer", "INTEGER[] NOT NULL")]
    [InlineData("BigInt", "BIGINT[] NOT NULL")]
    [InlineData("Real", "REAL[] NOT NULL")]
    [InlineData("DoublePrecision", "DOUBLE PRECISION[] NOT NULL")]
    [InlineData("Float", "FLOAT(24)[] NOT NULL")]
    [InlineData("Numeric", "NUMERIC[] NOT NULL")]
    [InlineData("NumericPrecision", "NUMERIC(18)[] NOT NULL")]
    [InlineData("NumericPrecisionScale", "NUMERIC(18, 2)[] NOT NULL")]
    [InlineData("Money", "MONEY[] NOT NULL")]
    [InlineData("Date", "DATE[] NOT NULL")]
    [InlineData("Time", "TIME[] NOT NULL")]
    [InlineData("TimePrecision", "TIME(6)[] NOT NULL")]
    [InlineData("TimeWithoutTimeZone", "TIME WITHOUT TIME ZONE[] NOT NULL")]
    [InlineData("TimeWithoutTimeZonePrecision", "TIME(6) WITHOUT TIME ZONE[] NOT NULL")]
    [InlineData("TimeWithTimeZone", "TIME WITH TIME ZONE[] NOT NULL")]
    [InlineData("TimeWithTimeZonePrecision", "TIME(6) WITH TIME ZONE[] NOT NULL")]
    [InlineData("Timestamp", "TIMESTAMP[] NOT NULL")]
    [InlineData("TimestampPrecision", "TIMESTAMP(6)[] NOT NULL")]
    [InlineData("TimestampWithoutTimeZone", "TIMESTAMP WITHOUT TIME ZONE[] NOT NULL")]
    [InlineData("TimestampWithoutTimeZonePrecision", "TIMESTAMP(6) WITHOUT TIME ZONE[] NOT NULL")]
    [InlineData("TimestampWithTimeZone", "TIMESTAMP WITH TIME ZONE[] NOT NULL")]
    [InlineData("TimestampWithTimeZonePrecision", "TIMESTAMP(6) WITH TIME ZONE[] NOT NULL")]
    [InlineData("Interval", "INTERVAL[] NOT NULL")]
    [InlineData("Xml", "XML[] NOT NULL")]
    [InlineData("Json", "JSON[] NOT NULL")]
    [InlineData("JsonB", "JSONB[] NOT NULL")]
    [InlineData("Bit", "BIT(8)[] NOT NULL")]
    [InlineData("VarBit", "VARBIT(8)[] NOT NULL")]
    [InlineData("Vector", "VECTOR(1536)[] NOT NULL")]
    public void ArrayFactories_ReturnArrayFieldProperties(string caseName, string expectedDeclaration)
    {
        FieldProperties actual = GetArrayFieldProperties(caseName);
        PostgreSqlDialect dialect = new();

        Assert.Equal((bool?)true, actual.IsArray);
        Assert.Equal(expectedDeclaration, dialect.RenderFieldProperties(actual));
    }

    [Fact]
    public void ArrayFactory_NullableTrue_ReturnsNullableArrayFieldProperties()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsInteger(true, true);
        PostgreSqlDialect dialect = new();

        Assert.Equal((bool?)true, actual.IsArray);
        Assert.True(actual.IsNullable);
        Assert.Equal("INTEGER[] NULL", dialect.RenderFieldProperties(actual));
    }

    private static FieldProperties GetArrayFieldProperties(string caseName) =>
        caseName switch
        {
            "Uuid" => PostgreSqlTypesProvider.AsUuid(true),
            "Char" => PostgreSqlTypesProvider.AsChar(10, true),
            "VarChar" => PostgreSqlTypesProvider.AsVarChar(50, true),
            "Text" => PostgreSqlTypesProvider.AsText(true),
            "Bytea" => PostgreSqlTypesProvider.AsBytea(true),
            "Boolean" => PostgreSqlTypesProvider.AsBoolean(true),
            "SmallInt" => PostgreSqlTypesProvider.AsSmallInt(true),
            "Integer" => PostgreSqlTypesProvider.AsInteger(true),
            "BigInt" => PostgreSqlTypesProvider.AsBigInt(true),
            "Real" => PostgreSqlTypesProvider.AsReal(true),
            "DoublePrecision" => PostgreSqlTypesProvider.AsDoublePrecision(true),
            "Float" => PostgreSqlTypesProvider.AsFloat(24, true),
            "Numeric" => PostgreSqlTypesProvider.AsNumeric(true),
            "NumericPrecision" => PostgreSqlTypesProvider.AsNumeric(18, true),
            "NumericPrecisionScale" => PostgreSqlTypesProvider.AsNumeric(18, 2, true),
            "Money" => PostgreSqlTypesProvider.AsMoney(true),
            "Date" => PostgreSqlTypesProvider.AsDate(true),
            "Time" => PostgreSqlTypesProvider.AsTime(true),
            "TimePrecision" => PostgreSqlTypesProvider.AsTime(6, true),
            "TimeWithoutTimeZone" => PostgreSqlTypesProvider.AsTimeWithoutTimeZone(true),
            "TimeWithoutTimeZonePrecision" => PostgreSqlTypesProvider.AsTimeWithoutTimeZone(6, true),
            "TimeWithTimeZone" => PostgreSqlTypesProvider.AsTimeWithTimeZone(true),
            "TimeWithTimeZonePrecision" => PostgreSqlTypesProvider.AsTimeWithTimeZone(6, true),
            "Timestamp" => PostgreSqlTypesProvider.AsTimestamp(true),
            "TimestampPrecision" => PostgreSqlTypesProvider.AsTimestamp(6, true),
            "TimestampWithoutTimeZone" => PostgreSqlTypesProvider.AsTimestampWithoutTimeZone(true),
            "TimestampWithoutTimeZonePrecision" => PostgreSqlTypesProvider.AsTimestampWithoutTimeZone(6, true),
            "TimestampWithTimeZone" => PostgreSqlTypesProvider.AsTimestampWithTimeZone(true),
            "TimestampWithTimeZonePrecision" => PostgreSqlTypesProvider.AsTimestampWithTimeZone(6, true),
            "Interval" => PostgreSqlTypesProvider.AsInterval(true),
            "Xml" => PostgreSqlTypesProvider.AsXml(true),
            "Json" => PostgreSqlTypesProvider.AsJson(true),
            "JsonB" => PostgreSqlTypesProvider.AsJsonB(true),
            "Bit" => PostgreSqlTypesProvider.AsBit(8, true),
            "VarBit" => PostgreSqlTypesProvider.AsVarBit(8, true),
            "Vector" => PostgreSqlTypesProvider.AsVector(1536, true),
            _ => throw new ArgumentOutOfRangeException(nameof(caseName), caseName, "Unsupported array field properties test case.")
        };
}
