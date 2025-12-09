using System.Data;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.TypesTests;

public sealed partial class SqlTypeDefinitionTests
{
    [Theory]
    [InlineData(typeof(Guid), SqlDbType.UniqueIdentifier, "UNIQUEIDENTIFIER")]
    [InlineData(typeof(Guid?), SqlDbType.UniqueIdentifier, "UNIQUEIDENTIFIER")]

    [InlineData(typeof(string), SqlDbType.NVarChar, "NVARCHAR(MAX)")]
    [InlineData(typeof(char), SqlDbType.NChar, "NCHAR(1)")]
    [InlineData(typeof(char?), SqlDbType.NChar, "NCHAR(1)")]

    [InlineData(typeof(byte[]), SqlDbType.VarBinary, "VARBINARY(MAX)")]

    [InlineData(typeof(bool), SqlDbType.Bit, "BIT")]

    [InlineData(typeof(byte), SqlDbType.TinyInt, "TINYINT")]
    [InlineData(typeof(byte?), SqlDbType.TinyInt, "TINYINT")]
    [InlineData(typeof(sbyte), SqlDbType.SmallInt, "SMALLINT")]
    [InlineData(typeof(sbyte?), SqlDbType.SmallInt, "SMALLINT")]
    [InlineData(typeof(short), SqlDbType.SmallInt, "SMALLINT")]
    [InlineData(typeof(short?), SqlDbType.SmallInt, "SMALLINT")]
    [InlineData(typeof(int), SqlDbType.Int, "INT")]
    [InlineData(typeof(int?), SqlDbType.Int, "INT")]
    [InlineData(typeof(long), SqlDbType.BigInt, "BIGINT")]
    [InlineData(typeof(long?), SqlDbType.BigInt, "BIGINT")]

    [InlineData(typeof(float), SqlDbType.Real, "REAL")]
    [InlineData(typeof(float?), SqlDbType.Real, "REAL")]
    [InlineData(typeof(double), SqlDbType.Float, "FLOAT")]
    [InlineData(typeof(double?), SqlDbType.Float, "FLOAT")]
    [InlineData(typeof(decimal), SqlDbType.Decimal, "DECIMAL")]
    [InlineData(typeof(decimal?), SqlDbType.Decimal, "DECIMAL")]

    [InlineData(typeof(DateTime), SqlDbType.DateTime2, "DATETIME2")]
    [InlineData(typeof(DateTime?), SqlDbType.DateTime2, "DATETIME2")]
    [InlineData(typeof(DateOnly), SqlDbType.Date, "DATE")]
    [InlineData(typeof(DateOnly?), SqlDbType.Date, "DATE")]
    [InlineData(typeof(TimeOnly), SqlDbType.Time, "TIME")]
    [InlineData(typeof(TimeOnly?), SqlDbType.Time, "TIME")]
    [InlineData(typeof(DateTimeOffset), SqlDbType.DateTimeOffset, "DATETIMEOFFSET")]
    [InlineData(typeof(DateTimeOffset?), SqlDbType.DateTimeOffset, "DATETIMEOFFSET")]

    [InlineData(typeof(EncodingEnum), SqlDbType.Int, "INT")]
    [InlineData(typeof(EncodingEnum?), SqlDbType.Int, "INT")]

    [InlineData(typeof(object), SqlDbType.Variant, "SQL_VARIANT")]
    public void TypeConstructor(Type inputType, SqlDbType expectedDbType, string expectedDeclaration)
    {
        SqlTypeDefinition definition = new(inputType);
        Assert.Equal(expectedDbType, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }



    [Theory]
    //xUnite doesn't support a Guid values as an argument for InlineData, because a Guid doesn't have constant expressions.
    //See ValueConstructor_Guid for Guid value test.

    //xUnite doesn't support a decimal value as an argument for InlineData, because a decimal literal isn't a constant expression?
    //See ValueConstructor_Decimal for decimal value test.

    //xUnite doesn't support a DateTime value as an argument for InlineData, because a DateTime doesn't have constant expressions.
    //See ValueConstructor_DateTime for DateTime value test.

    //xUnite doesn't support a DateOnly value as an argument for InlineData, because a DateOnly doesn't have constant expressions.
    //See ValueConstructor_Date for DateOnly value test.

    //xUnite doesn't support a TimeOnly value as an argument for InlineData, because a TimeOnly doesn't have constant expressions.
    //See ValueConstructor_Time for TimeOnly value test.

    //xUnite doesn't support a DateTimeOffset value as an argument for InlineData, because a DateTimeOffset doesn't have constant expressions.
    //See ValueConstructor_DateTimeOffset for DateTimeOffset value test.

    [InlineData("Hello World!", SqlDbType.NVarChar, "NVARCHAR(MAX)")]
    [InlineData('X', SqlDbType.NChar, "NCHAR(1)")]

    [InlineData(new byte[] { 0x32 }, SqlDbType.VarBinary, "VARBINARY(MAX)")]

    [InlineData(true, SqlDbType.Bit, "BIT")]

    [InlineData(byte.MaxValue, SqlDbType.TinyInt, "TINYINT")]
    [InlineData((sbyte)42, SqlDbType.SmallInt, "SMALLINT")]
    [InlineData((short)1337, SqlDbType.SmallInt, "SMALLINT")]
    [InlineData(1701, SqlDbType.Int, "INT")]
    [InlineData(32423423324456894L, SqlDbType.BigInt, "BIGINT")]

    [InlineData(float.Pi, SqlDbType.Real, "REAL")]
    [InlineData(double.E, SqlDbType.Float, "FLOAT")]

    [InlineData(EncodingEnum.Ascii, SqlDbType.Int, "INT")]

    [InlineData(null, SqlDbType.Variant, "SQL_VARIANT")]
    public void ValueConstructor(object? value, SqlDbType expectedDbType, string expectedDeclaration)
    {
        SqlTypeDefinition definition = new(value);
        Assert.Equal(expectedDbType, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }

    [Fact]
    public void ValueConstructor_Guid()
    {
        object value = Guid.Empty;
        SqlDbType expectedDbType = SqlDbType.UniqueIdentifier;
        string expectedDeclaration = "UNIQUEIDENTIFIER";
        SqlTypeDefinition definition = new(value);
        Assert.Equal(expectedDbType, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }

    [Fact]
    public void ValueConstructor_Decimal()
    {
        object value = 1.61803398875m;
        SqlDbType expectedDbType = SqlDbType.Decimal;
        string expectedDeclaration = "DECIMAL";
        SqlTypeDefinition definition = new(value);
        Assert.Equal(expectedDbType, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }

    [Fact]
    public void ValueConstructor_DateTime()
    {
        object value = DateTime.Now;
        SqlDbType expectedDbType = SqlDbType.DateTime2;
        string expectedDeclaration = "DATETIME2";
        SqlTypeDefinition definition = new(value);
        Assert.Equal(expectedDbType, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }

    [Fact]
    public void ValueConstructor_Date()
    {
        object value = new DateOnly(1989, 04, 24);
        SqlDbType expectedDbType = SqlDbType.Date;
        string expectedDeclaration = "DATE";
        SqlTypeDefinition definition = new(value);
        Assert.Equal(expectedDbType, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }

    [Fact]
    public void ValueConstructor_Time()
    {
        object value = new TimeOnly(9, 16);
        SqlDbType expectedDbType = SqlDbType.Time;
        string expectedDeclaration = "TIME";
        SqlTypeDefinition definition = new(value);
        Assert.Equal(expectedDbType, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }

    [Fact]
    public void ValueConstructor_DateTimeOffset()
    {
        object value = DateTimeOffset.Now;
        SqlDbType expectedDbType = SqlDbType.DateTimeOffset;
        string expectedDeclaration = "DATETIMEOFFSET";
        SqlTypeDefinition definition = new(value);
        Assert.Equal(expectedDbType, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }

    [Fact]
    public void ValueConstructor_Object()
    {
        object value = new object();
        SqlDbType expectedDbType = SqlDbType.Variant;
        string expectedDeclaration = "SQL_VARIANT";
        SqlTypeDefinition definition = new(value);
        Assert.Equal(expectedDbType, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }
}
