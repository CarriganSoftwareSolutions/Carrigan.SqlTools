using Carrigan.SqlTools.Types;
using Xunit;

namespace Carrigan.SqlTools.Tests.TypesTests;
//IGNORE SPELLING: bigint
public class FieldPropertiesTests
{
    [Fact]
    public void ToString_ReturnsEmptyString_WhenProviderTypeNameIsNull() =>
        Assert.Equal(string.Empty, new FieldProperties().ToString());

    [Fact]
    public void ToString_ReturnsNotNullDeclaration_WhenNullableIsFalse() =>
        Assert.Equal("INT NOT NULL", new FieldProperties { ProviderTypeName = "INT" }.ToString());

    [Fact]
    public void ToString_ReturnsNullDeclaration_WhenNullableIsTrue() =>
        Assert.Equal("INT NULL", new FieldProperties { ProviderTypeName = "INT", IsNullable = true }.ToString());

    [Fact]
    public void ToString_ReturnsLengthDeclaration_ForNVarChar() => 
        Assert.Equal("NVARCHAR(100) NOT NULL", new FieldProperties { ProviderTypeName = "NVARCHAR", Length = 100 }.ToString());

    [Fact]
    public void ToString_ReturnsMaxDeclaration_WhenIsMaxTrue() => 
        Assert.Equal("NVARCHAR(MAX) NOT NULL", new FieldProperties { ProviderTypeName = "NVARCHAR", IsMax = true }.ToString());

    [Fact]
    public void ToString_ReturnsPrecisionAndScaleDeclaration_WhenPrecisionAndScaleAreSpecified() =>
        Assert.Equal("DECIMAL(18, 2) NOT NULL", new FieldProperties { ProviderTypeName = "DECIMAL", Precision = 18, Scale = 2 }.ToString());

    [Fact]
    public void ToString_ReturnsPrecisionDeclaration_WhenOnlyPrecisionIsSpecified() =>
        Assert.Equal("FLOAT(24) NOT NULL", new FieldProperties { ProviderTypeName = "FLOAT", Precision = 24 }.ToString());

    [Fact]
    public void ToString_ReturnsFractionalSecondsPrecisionDeclaration_WhenFractionalSecondsPrecisionIsSpecified() =>
        Assert.Equal("DATETIME2(7) NOT NULL", new FieldProperties { ProviderTypeName = "DATETIME2", FractionalSecondsPrecision = 7 }.ToString());

    [Fact]
    public void ToString_ReturnsUppercaseDeclaration_WhenProviderTypeNameIsLowercase() =>
        Assert.Equal("BIGINT NOT NULL", new FieldProperties { ProviderTypeName = "bigint" }.ToString());

    [Fact]
    public void ToString_ReturnsVectorLengthDeclaration() =>
        Assert.Equal("VECTOR(3) NOT NULL", new FieldProperties { ProviderTypeName = "VECTOR", Length = 3 }.ToString());

    [Fact]
    public void ToString_ReturnsVectorBaseTypeDeclaration() =>
        Assert.Equal("VECTOR(3, FLOAT16) NOT NULL", new FieldProperties { ProviderTypeName = "VECTOR", Length = 3, BaseType = "FLOAT16" }.ToString());
}