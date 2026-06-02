using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Theory]
    [InlineData(typeof(Guid), "UUID", false, false)]
    [InlineData(typeof(Guid?), "UUID", true, false)]
    [InlineData(typeof(Guid[]), "UUID", false, true)]
    [InlineData(typeof(Guid?[]), "UUID", false, true)]
    [InlineData(typeof(string), "TEXT", false, false)]
    [InlineData(typeof(string[]), "TEXT", false, true)]
    [InlineData(typeof(char), "CHAR", false, false)]
    [InlineData(typeof(char?), "CHAR", true, false)]
    [InlineData(typeof(char[]), "CHAR", false, true)]
    [InlineData(typeof(char?[]), "CHAR", false, true)]
    [InlineData(typeof(byte[]), "BYTEA", false, false)]
    [InlineData(typeof(byte[][]), "BYTEA", false, true)]
    [InlineData(typeof(bool), "BOOLEAN", false, false)]
    [InlineData(typeof(bool?), "BOOLEAN", true, false)]
    [InlineData(typeof(bool[]), "BOOLEAN", false, true)]
    [InlineData(typeof(bool?[]), "BOOLEAN", false, true)]
    [InlineData(typeof(byte), "SMALLINT", false, false)]
    [InlineData(typeof(byte?), "SMALLINT", true, false)]
    [InlineData(typeof(byte?[]), "SMALLINT", false, true)]
    [InlineData(typeof(sbyte), "SMALLINT", false, false)]
    [InlineData(typeof(sbyte?), "SMALLINT", true, false)]
    [InlineData(typeof(sbyte[]), "SMALLINT", false, true)]
    [InlineData(typeof(sbyte?[]), "SMALLINT", false, true)]
    [InlineData(typeof(short), "SMALLINT", false, false)]
    [InlineData(typeof(short?), "SMALLINT", true, false)]
    [InlineData(typeof(short[]), "SMALLINT", false, true)]
    [InlineData(typeof(short?[]), "SMALLINT", false, true)]
    [InlineData(typeof(ushort), "INTEGER", false, false)]
    [InlineData(typeof(ushort?), "INTEGER", true, false)]
    [InlineData(typeof(ushort[]), "INTEGER", false, true)]
    [InlineData(typeof(ushort?[]), "INTEGER", false, true)]
    [InlineData(typeof(int), "INTEGER", false, false)]
    [InlineData(typeof(int?), "INTEGER", true, false)]
    [InlineData(typeof(int[]), "INTEGER", false, true)]
    [InlineData(typeof(int?[]), "INTEGER", false, true)]
    [InlineData(typeof(uint), "BIGINT", false, false)]
    [InlineData(typeof(uint?), "BIGINT", true, false)]
    [InlineData(typeof(uint[]), "BIGINT", false, true)]
    [InlineData(typeof(uint?[]), "BIGINT", false, true)]
    [InlineData(typeof(long), "BIGINT", false, false)]
    [InlineData(typeof(long?), "BIGINT", true, false)]
    [InlineData(typeof(long[]), "BIGINT", false, true)]
    [InlineData(typeof(long?[]), "BIGINT", false, true)]
    [InlineData(typeof(ulong), "NUMERIC", false, false)]
    [InlineData(typeof(ulong?), "NUMERIC", true, false)]
    [InlineData(typeof(ulong[]), "NUMERIC", false, true)]
    [InlineData(typeof(ulong?[]), "NUMERIC", false, true)]
    [InlineData(typeof(float), "REAL", false, false)]
    [InlineData(typeof(float?), "REAL", true, false)]
    [InlineData(typeof(float[]), "REAL", false, true)]
    [InlineData(typeof(float?[]), "REAL", false, true)]
    [InlineData(typeof(double), "DOUBLE PRECISION", false, false)]
    [InlineData(typeof(double?), "DOUBLE PRECISION", true, false)]
    [InlineData(typeof(double[]), "DOUBLE PRECISION", false, true)]
    [InlineData(typeof(double?[]), "DOUBLE PRECISION", false, true)]
    [InlineData(typeof(decimal), "NUMERIC", false, false)]
    [InlineData(typeof(decimal?), "NUMERIC", true, false)]
    [InlineData(typeof(decimal[]), "NUMERIC", false, true)]
    [InlineData(typeof(decimal?[]), "NUMERIC", false, true)]
    [InlineData(typeof(DateTime), "TIMESTAMP WITHOUT TIME ZONE", false, false)]
    [InlineData(typeof(DateTime?), "TIMESTAMP WITHOUT TIME ZONE", true, false)]
    [InlineData(typeof(DateTime[]), "TIMESTAMP WITHOUT TIME ZONE", false, true)]
    [InlineData(typeof(DateTime?[]), "TIMESTAMP WITHOUT TIME ZONE", false, true)]
    [InlineData(typeof(DateOnly), "DATE", false, false)]
    [InlineData(typeof(DateOnly?), "DATE", true, false)]
    [InlineData(typeof(DateOnly[]), "DATE", false, true)]
    [InlineData(typeof(DateOnly?[]), "DATE", false, true)]
    [InlineData(typeof(TimeOnly), "TIME WITHOUT TIME ZONE", false, false)]
    [InlineData(typeof(TimeOnly?), "TIME WITHOUT TIME ZONE", true, false)]
    [InlineData(typeof(TimeOnly[]), "TIME WITHOUT TIME ZONE", false, true)]
    [InlineData(typeof(TimeOnly?[]), "TIME WITHOUT TIME ZONE", false, true)]
    [InlineData(typeof(TimeSpan), "INTERVAL", false, false)]
    [InlineData(typeof(TimeSpan?), "INTERVAL", true, false)]
    [InlineData(typeof(TimeSpan[]), "INTERVAL", false, true)]
    [InlineData(typeof(TimeSpan?[]), "INTERVAL", false, true)]
    [InlineData(typeof(DateTimeOffset), "TIMESTAMP WITH TIME ZONE", false, false)]
    [InlineData(typeof(DateTimeOffset?), "TIMESTAMP WITH TIME ZONE", true, false)]
    [InlineData(typeof(DateTimeOffset[]), "TIMESTAMP WITH TIME ZONE", false, true)]
    [InlineData(typeof(DateTimeOffset?[]), "TIMESTAMP WITH TIME ZONE", false, true)]
    [InlineData(typeof(XmlDocument), "XML", false, false)]
    [InlineData(typeof(XDocument), "XML", false, false)]
    [InlineData(typeof(XmlDocument[]), "XML", false, true)]
    [InlineData(typeof(XDocument[]), "XML", false, true)]
    [InlineData(typeof(object), "TEXT", false, false)]
    [InlineData(typeof(object[]), "TEXT", false, true)]
    public void FromClrType_ReturnsExpectedProviderTypeNameNullabilityAndArrayFlag(Type clrType, string expectedProviderTypeName, bool expectedNullable, bool expectedIsArray)
    {
        FieldProperties actual = PostgreSqlTypesProvider.FromClrType(clrType);

        Assert.Equal(expectedProviderTypeName, actual.ProviderTypeName);
        Assert.Equal(expectedNullable, actual.IsNullable);
        Assert.Equal((bool?)expectedIsArray, actual.IsArray);
    }

    [Theory]
    [InlineData(typeof(ulong), false)]
    [InlineData(typeof(ulong?), false)]
    [InlineData(typeof(ulong[]), true)]
    [InlineData(typeof(ulong?[]), true)]
    public void FromClrType_UlongTypes_ReturnsNumericTwentyZero(Type clrType, bool expectedIsArray)
    {
        FieldProperties actual = PostgreSqlTypesProvider.FromClrType(clrType);

        Assert.Equal("NUMERIC", actual.ProviderTypeName);
        Assert.Equal((byte)20, actual.Precision);
        Assert.Equal((byte)0, actual.Scale);
        Assert.Equal((bool?)expectedIsArray, actual.IsArray);
    }

    [Fact]
    public void FromClrType_NullType_Exception() =>
        Assert.Throws<ArgumentNullException>(() => PostgreSqlTypesProvider.FromClrType(null!));
}
