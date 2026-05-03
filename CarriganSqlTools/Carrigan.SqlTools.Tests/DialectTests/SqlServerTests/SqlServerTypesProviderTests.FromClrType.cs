using System;
using System.Xml;
using System.Xml.Linq;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Types;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Theory]
    [InlineData(typeof(Guid), "UNIQUEIDENTIFIER", false)]
    [InlineData(typeof(string), "NVARCHAR", false)]
    [InlineData(typeof(char), "NCHAR", false)]
    [InlineData(typeof(byte[]), "VARBINARY", false)]
    [InlineData(typeof(bool), "BIT", false)]
    [InlineData(typeof(byte), "TINYINT", false)]
    [InlineData(typeof(sbyte), "SMALLINT", false)]
    [InlineData(typeof(short), "SMALLINT", false)]
    [InlineData(typeof(ushort), "INT", false)]
    [InlineData(typeof(int), "INT", false)]
    [InlineData(typeof(uint), "BIGINT", false)]
    [InlineData(typeof(long), "BIGINT", false)]
    [InlineData(typeof(float), "REAL", false)]
    [InlineData(typeof(double), "FLOAT", false)]
    [InlineData(typeof(decimal), "DECIMAL", false)]
    [InlineData(typeof(DateTime), "DATETIME2", false)]
    [InlineData(typeof(DateOnly), "DATE", false)]
    [InlineData(typeof(TimeOnly), "TIME", false)]
    [InlineData(typeof(DateTimeOffset), "DATETIMEOFFSET", false)]
    [InlineData(typeof(XmlDocument), "XML", false)]
    [InlineData(typeof(XDocument), "XML", false)]
    public void FromClrType_ReturnsExpectedMapping(Type clrType, string expectedProviderTypeName, bool expectedNullable)
    {
        FieldProperties actual = SqlServerTypesProvider.FromClrType(clrType);

        Assert.Equal(expectedProviderTypeName, actual.ProviderTypeName);
        Assert.Equal(expectedNullable, actual.IsNullable);
    }

    [Theory]
    [InlineData(typeof(bool?), "BIT")]
    [InlineData(typeof(byte?), "TINYINT")]
    [InlineData(typeof(short?), "SMALLINT")]
    [InlineData(typeof(int?), "INT")]
    [InlineData(typeof(long?), "BIGINT")]
    [InlineData(typeof(float?), "REAL")]
    [InlineData(typeof(double?), "FLOAT")]
    [InlineData(typeof(decimal?), "DECIMAL")]
    [InlineData(typeof(DateTime?), "DATETIME2")]
    [InlineData(typeof(DateOnly?), "DATE")]
    [InlineData(typeof(TimeOnly?), "TIME")]
    [InlineData(typeof(DateTimeOffset?), "DATETIMEOFFSET")]
    [InlineData(typeof(Guid?), "UNIQUEIDENTIFIER")]
    public void FromClrType_ReturnsNullable_WhenClrTypeIsNullableValueType(Type clrType, string expectedProviderTypeName)
    {
        FieldProperties actual = SqlServerTypesProvider.FromClrType(clrType);

        Assert.Equal(expectedProviderTypeName, actual.ProviderTypeName);
        Assert.True(actual.IsNullable);
    }

    [Fact]
    public void FromClrType_Generic_ReturnsExpectedMapping()
    {
        FieldProperties actual = SqlServerTypesProvider.FromClrType<int>();

        Assert.Equal("INT", actual.ProviderTypeName);
        Assert.False(actual.IsNullable);
    }

    [Fact]
    public void FromNullableClrType_ReturnsNullable()
    {
        FieldProperties actual = SqlServerTypesProvider.FromNullableClrType(typeof(string));

        Assert.Equal("NVARCHAR", actual.ProviderTypeName);
        Assert.True(actual.IsNullable);
    }

    [Fact]
    public void FromNullableClrType_Generic_ReturnsNullable()
    {
        FieldProperties actual = SqlServerTypesProvider.FromNullableClrType<string>();

        Assert.Equal("NVARCHAR", actual.ProviderTypeName);
        Assert.True(actual.IsNullable);
    }

    [Fact]
    public void FromClrType_ReturnsSqlVariant_WhenClrTypeIsNotMapped()
    {
        FieldProperties actual = SqlServerTypesProvider.FromClrType(typeof(Uri));

        Assert.Equal("SQL_VARIANT", actual.ProviderTypeName);
        Assert.False(actual.IsNullable);
    }

    [Fact]
    public void FromClrType_MapsUnsignedLongToDecimal20Scale0()
    {
        FieldProperties actual = SqlServerTypesProvider.FromClrType(typeof(ulong));

        Assert.Equal("DECIMAL", actual.ProviderTypeName);
        Assert.Equal((byte)20, actual.Precision);
        Assert.Equal((byte)0, actual.Scale);
    }
}
