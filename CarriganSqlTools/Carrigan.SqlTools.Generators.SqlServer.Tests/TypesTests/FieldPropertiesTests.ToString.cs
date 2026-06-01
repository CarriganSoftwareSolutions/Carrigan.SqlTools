using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.TypesTests;
//IGNORE SPELLING: bigint
public class FieldPropertiesTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void ToString_ReturnsEmptyString_WhenProviderTypeNameIsNull() =>
        Assert.Equal(string.Empty, Dialect.RenderFieldProperties(new FieldProperties()));

    [Fact]
    public void ToString_ReturnsNotNullDeclaration_WhenNullableIsFalse() =>
        Assert.Equal("INT NOT NULL", Dialect.RenderFieldProperties(new FieldProperties { ProviderTypeName = "INT" }));

    [Fact]
    public void ToString_ReturnsNullDeclaration_WhenNullableIsTrue() =>
        Assert.Equal("INT NULL", Dialect.RenderFieldProperties(new FieldProperties { ProviderTypeName = "INT", IsNullable = true }));

    [Fact]
    public void ToString_ReturnsLengthDeclaration_ForNVarChar() => 
        Assert.Equal("NVARCHAR(100) NOT NULL", Dialect.RenderFieldProperties(new FieldProperties { ProviderTypeName = "NVARCHAR", Length = 100 }));

    [Fact]
    public void ToString_ReturnsMaxDeclaration_WhenIsMaxTrue() => 
        Assert.Equal("NVARCHAR(MAX) NOT NULL", Dialect.RenderFieldProperties(new FieldProperties { ProviderTypeName = "NVARCHAR", IsMax = true }));

    [Fact]
    public void ToString_ReturnsPrecisionAndScaleDeclaration_WhenPrecisionAndScaleAreSpecified() =>
        Assert.Equal("DECIMAL(18, 2) NOT NULL", Dialect.RenderFieldProperties(new FieldProperties { ProviderTypeName = "DECIMAL", Precision = 18, Scale = 2 }));

    [Fact]
    public void ToString_ReturnsPrecisionDeclaration_WhenOnlyPrecisionIsSpecified() =>
        Assert.Equal("FLOAT(24) NOT NULL", Dialect.RenderFieldProperties(new FieldProperties { ProviderTypeName = "FLOAT", Precision = 24 }));

    [Fact]
    public void ToString_ReturnsFractionalSecondsPrecisionDeclaration_WhenFractionalSecondsPrecisionIsSpecified() =>
        Assert.Equal("DATETIME2(7) NOT NULL", Dialect.RenderFieldProperties(new FieldProperties { ProviderTypeName = "DATETIME2", FractionalSecondsPrecision = 7 }));

    [Fact]
    public void ToString_ReturnsUppercaseDeclaration_WhenProviderTypeNameIsLowercase() =>
        Assert.Equal("BIGINT NOT NULL", Dialect.RenderFieldProperties(new FieldProperties { ProviderTypeName = "bigint" }));

    [Fact]
    public void ToString_ReturnsVectorLengthDeclaration() =>
        Assert.Equal("VECTOR(3) NOT NULL", Dialect.RenderFieldProperties(new FieldProperties { ProviderTypeName = "VECTOR", Length = 3 }));

    [Fact]
    public void ToString_ReturnsVectorBaseTypeDeclaration() =>
        Assert.Equal("VECTOR(3, FLOAT16) NOT NULL", Dialect.RenderFieldProperties(new FieldProperties { ProviderTypeName = "VECTOR", Length = 3, BaseType = "FLOAT16" }));
}