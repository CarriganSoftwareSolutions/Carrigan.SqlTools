using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.SqlServer.Tests;

public class SqlQueryExtensionsTests
{
    [Fact]
    public void GetParameterCollection_ExplicitNVarCharMax_ReturnsExpectedSqlParameter()
    {
        FieldProperties fieldProperties = new()
        {
            ProviderTypeName = "NVARCHAR",
            IsMax = true,
            IsNullable = true
        };

        SqlParameter actual = GetSingleParameter("Name", "Test", fieldProperties);

        Assert.Equal("@Name_1", actual.ParameterName);
        Assert.Equal("Test", actual.Value);
        Assert.Equal(SqlDbType.NVarChar, actual.SqlDbType);
        Assert.Equal(-1, actual.Size);
        Assert.True(actual.IsNullable);
    }

    [Fact]
    public void GetParameterCollection_ExplicitVarCharLength_ReturnsExpectedSize()
    {
        FieldProperties fieldProperties = new()
        {
            ProviderTypeName = "VARCHAR",
            Length = 50,
            IsNullable = false
        };

        SqlParameter actual = GetSingleParameter("Name", "Test", fieldProperties);

        Assert.Equal("@Name_1", actual.ParameterName);
        Assert.Equal("Test", actual.Value);
        Assert.Equal(SqlDbType.VarChar, actual.SqlDbType);
        Assert.Equal(50, actual.Size);
        Assert.False(actual.IsNullable);
    }

    [Fact]
    public void GetParameterCollection_ExplicitDecimal_ReturnsExpectedPrecisionAndScale()
    {
        FieldProperties fieldProperties = new()
        {
            ProviderTypeName = "DECIMAL",
            Precision = 18,
            Scale = 4
        };

        SqlParameter actual = GetSingleParameter("Amount", 123.45m, fieldProperties);

        Assert.Equal("@Amount_1", actual.ParameterName);
        Assert.Equal(123.45m, actual.Value);
        Assert.Equal(SqlDbType.Decimal, actual.SqlDbType);
        Assert.Equal(18, actual.Precision);
        Assert.Equal(4, actual.Scale);
    }

    [Fact]
    public void GetParameterCollection_ExplicitTime_ReturnsExpectedFractionalSecondsPrecisionAsScale()
    {
        FieldProperties fieldProperties = new()
        {
            ProviderTypeName = "TIME",
            FractionalSecondsPrecision = 7
        };

        TimeSpan value = TimeSpan.FromMinutes(5);

        SqlParameter actual = GetSingleParameter("StartTime", value, fieldProperties);

        Assert.Equal("@StartTime_1", actual.ParameterName);
        Assert.Equal(value, actual.Value);
        Assert.Equal(SqlDbType.Time, actual.SqlDbType);
        Assert.Equal(7, actual.Scale);
    }

    [Fact]
    public void GetParameterCollection_NullValue_ConvertsToDBNull()
    {
        FieldProperties fieldProperties = new()
        {
            ProviderTypeName = "INT",
            IsNullable = true
        };

        SqlParameter actual = GetSingleParameter("Id", null, fieldProperties);

        Assert.Equal("@Id_1", actual.ParameterName);
        Assert.Equal(DBNull.Value, actual.Value);
        Assert.Equal(SqlDbType.Int, actual.SqlDbType);
        Assert.True(actual.IsNullable);
    }

    [Fact]
    public void GetParameterCollection_XDocumentValue_ConvertsToString()
    {
        FieldProperties fieldProperties = new()
        {
            ProviderTypeName = "XML"
        };

        XDocument document = new(new XElement("root", new XElement("value", "test")));

        SqlParameter actual = GetSingleParameter("XmlValue", document, fieldProperties);

        Assert.Equal("@XmlValue_1", actual.ParameterName);
        Assert.Equal(SqlDbType.Xml, actual.SqlDbType);
        Assert.IsType<string>(actual.Value);
        Assert.Contains("<root>", actual.Value.ToString());
    }

    [Fact]
    public void GetParameterCollection_XmlDocumentValue_ConvertsToOuterXml()
    {
        FieldProperties fieldProperties = new()
        {
            ProviderTypeName = "XML"
        };

        XmlDocument document = new();
        document.LoadXml("<root><value>test</value></root>");

        SqlParameter actual = GetSingleParameter("XmlValue", document, fieldProperties);

        Assert.Equal("@XmlValue_1", actual.ParameterName);
        Assert.Equal(SqlDbType.Xml, actual.SqlDbType);
        Assert.Equal("<root><value>test</value></root>", actual.Value);
    }

    [Theory]
    [InlineData("BIGINT", SqlDbType.BigInt)]
    [InlineData("BINARY", SqlDbType.Binary)]
    [InlineData("BIT", SqlDbType.Bit)]
    [InlineData("CHAR", SqlDbType.Char)]
    [InlineData("CURSOR", SqlDbType.Variant)]
    [InlineData("DATE", SqlDbType.Date)]
    [InlineData("DATETIME", SqlDbType.DateTime)]
    [InlineData("DATETIME2", SqlDbType.DateTime2)]
    [InlineData("DATETIMEOFFSET", SqlDbType.DateTimeOffset)]
    [InlineData("DECIMAL", SqlDbType.Decimal)]
    [InlineData("FLOAT", SqlDbType.Float)]
    [InlineData("GEOGRAPHY", SqlDbType.Udt)]
    [InlineData("GEOMETRY", SqlDbType.Udt)]
    [InlineData("HIERARCHYID", SqlDbType.Udt)]
    [InlineData("IMAGE", SqlDbType.Image)]
    [InlineData("INT", SqlDbType.Int)]
    [InlineData("JSON", SqlDbType.NVarChar)]
    [InlineData("MONEY", SqlDbType.Money)]
    [InlineData("NCHAR", SqlDbType.NChar)]
    [InlineData("NTEXT", SqlDbType.NText)]
    [InlineData("NUMERIC", SqlDbType.Decimal)]
    [InlineData("NVARCHAR", SqlDbType.NVarChar)]
    [InlineData("REAL", SqlDbType.Real)]
    [InlineData("ROWVERSION", SqlDbType.Timestamp)]
    [InlineData("SMALLDATETIME", SqlDbType.SmallDateTime)]
    [InlineData("SMALLINT", SqlDbType.SmallInt)]
    [InlineData("SMALLMONEY", SqlDbType.SmallMoney)]
    [InlineData("SQL_VARIANT", SqlDbType.Variant)]
    [InlineData("TABLE", SqlDbType.Structured)]
    [InlineData("TEXT", SqlDbType.Text)]
    [InlineData("TIME", SqlDbType.Time)]
    [InlineData("TIMESTAMP", SqlDbType.Timestamp)]
    [InlineData("TINYINT", SqlDbType.TinyInt)]
    [InlineData("UNIQUEIDENTIFIER", SqlDbType.UniqueIdentifier)]
    [InlineData("VARBINARY", SqlDbType.VarBinary)]
    [InlineData("VARCHAR", SqlDbType.VarChar)]
    [InlineData("VECTOR", SqlDbType.VarBinary)]
    [InlineData("XML", SqlDbType.Xml)]
    public void GetParameterCollection_ExplicitProviderTypeName_ReturnsExpectedSqlDbType(string providerTypeName, SqlDbType expected)
    {
        FieldProperties fieldProperties = new()
        {
            ProviderTypeName = providerTypeName
        };

        SqlParameter actual = GetSingleParameter("Value", "test", fieldProperties);

        Assert.Equal("@Value_1", actual.ParameterName);
        Assert.Equal(expected, actual.SqlDbType);
    }

    [Fact]
    public void GetParameterCollection_UnsupportedProviderTypeName_ThrowsArgumentOutOfRangeException()
    {
        FieldProperties fieldProperties = new()
        {
            ProviderTypeName = "NOT_A_REAL_TYPE"
        };

        SqlQuery query = GetQuery("Value", "test", fieldProperties);

        Assert.Throws<ArgumentOutOfRangeException>(() => query.GetParameterCollection().ToList());
    }

    private static SqlParameter GetSingleParameter(string parameterName, object? value, FieldProperties fieldProperties) =>
        GetQuery(parameterName, value, fieldProperties).GetParameterCollection().Single();

    private static SqlQuery GetQuery(string parameterName, object? value, FieldProperties fieldProperties)
    {
        Parameter parameter = new(parameterName, value);
        SqlFragmentParameter sqlFragmentParameter = new(new ParameterTag(parameterName), fieldProperties, value);
        SqlServerDialect dialect = new();

        return new (dialect, CommandType.Text, [sqlFragmentParameter]);
    }
}