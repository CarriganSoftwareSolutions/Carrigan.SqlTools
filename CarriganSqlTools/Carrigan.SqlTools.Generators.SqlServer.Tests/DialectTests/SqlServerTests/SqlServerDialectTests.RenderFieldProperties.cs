using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void RenderFieldProperties_ReturnsEmptyString_WhenProviderTypeNameIsNull()
    {
        string actual = Dialect.RenderFieldProperties(new FieldProperties());

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void RenderFieldProperties_ReturnsEmptyString_WhenProviderTypeNameIsWhitespace()
    {
        string actual = Dialect.RenderFieldProperties(new FieldProperties { ProviderTypeName = "   " });

        Assert.Equal(string.Empty, actual);
    }

    [Theory]
    [InlineData("INT", false, "INT NOT NULL")]
    [InlineData("INT", true, "INT NULL")]
    [InlineData("bigint", false, "BIGINT NOT NULL")]
    public void RenderFieldProperties_ReturnsExpectedNullabilityDeclaration(string providerTypeName, bool isNullable, string expected)
    {
        string actual = Dialect.RenderFieldProperties(new FieldProperties
        {
            ProviderTypeName = providerTypeName,
            IsNullable = isNullable
        });

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("CHAR", 10, "CHAR(10) NOT NULL")]
    [InlineData("VARCHAR", 50, "VARCHAR(50) NOT NULL")]
    [InlineData("NCHAR", 10, "NCHAR(10) NOT NULL")]
    [InlineData("NVARCHAR", 100, "NVARCHAR(100) NOT NULL")]
    [InlineData("BINARY", 16, "BINARY(16) NOT NULL")]
    [InlineData("VARBINARY", 256, "VARBINARY(256) NOT NULL")]
    public void RenderFieldProperties_ReturnsLengthDeclaration_ForLengthBasedTypes(string providerTypeName, int length, string expected)
    {
        string actual = Dialect.RenderFieldProperties(new FieldProperties
        {
            ProviderTypeName = providerTypeName,
            Length = length
        });

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RenderFieldProperties_ReturnsMaxDeclaration_WhenIsMaxIsTrue()
    {
        string actual = Dialect.RenderFieldProperties(new FieldProperties
        {
            ProviderTypeName = "NVARCHAR",
            IsMax = true
        });

        Assert.Equal("NVARCHAR(MAX) NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_ReturnsPrecisionAndScaleDeclaration_WhenBothAreSpecified()
    {
        string actual = Dialect.RenderFieldProperties(new FieldProperties
        {
            ProviderTypeName = "DECIMAL",
            Precision = 18,
            Scale = 2
        });

        Assert.Equal("DECIMAL(18, 2) NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_ReturnsPrecisionDeclaration_WhenOnlyPrecisionIsSpecified()
    {
        string actual = Dialect.RenderFieldProperties(new FieldProperties
        {
            ProviderTypeName = "FLOAT",
            Precision = 24
        });

        Assert.Equal("FLOAT(24) NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_ReturnsFractionalSecondsPrecisionDeclaration()
    {
        string actual = Dialect.RenderFieldProperties(new FieldProperties
        {
            ProviderTypeName = "DATETIME2",
            FractionalSecondsPrecision = 7
        });

        Assert.Equal("DATETIME2(7) NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_ReturnsVectorLengthDeclaration_WhenBaseTypeIsMissing()
    {
        string actual = Dialect.RenderFieldProperties(new FieldProperties
        {
            ProviderTypeName = "VECTOR",
            Length = 3
        });

        Assert.Equal("VECTOR(3) NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_ReturnsVectorLengthAndBaseTypeDeclaration_WhenBaseTypeIsSpecified()
    {
        string actual = Dialect.RenderFieldProperties(new FieldProperties
        {
            ProviderTypeName = "VECTOR",
            Length = 3,
            BaseType = "float16"
        });

        Assert.Equal("VECTOR(3, FLOAT16) NOT NULL", actual);
    }
}