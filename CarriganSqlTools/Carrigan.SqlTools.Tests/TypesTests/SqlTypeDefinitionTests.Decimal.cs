using System.Data;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.TypesTests;

public sealed partial class SqlTypeDefinitionTests
{
    [Theory]
    [InlineData(1, "DECIMAL(1)")]
    [InlineData(18, "DECIMAL(18)")]
    [InlineData(38, "DECIMAL(38)")]
    public void AsDecimal_WithPrecisionOnly(byte precision, string expectedDeclaration)
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsDecimal(precision);
        Assert.Equal(SqlDbType.Decimal, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
        Assert.Equal(precision, definition.Precision);
        Assert.Null(definition.Scale);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(39)]
    public void AsDecimal_WithPrecisionOnly_OutOfRange(byte precision) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsDecimal(precision));

    [Theory]
    [InlineData(18, 0, "DECIMAL(18, 0)")]
    [InlineData(18, 2, "DECIMAL(18, 2)")]
    [InlineData(10, 10, "DECIMAL(10, 10)")]
    [InlineData(16, 8, "DECIMAL(16, 8)")]
    [InlineData(38, 38, "DECIMAL(38, 38)")]
    public void AsDecimal_WithPrecisionAndScale(byte precision, byte scale, string expectedDeclaration)
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsDecimal(precision, scale);
        Assert.Equal(SqlDbType.Decimal, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
        Assert.Equal(precision, definition.Precision);
        Assert.Equal(scale, definition.Scale);
    }

    [Theory]
    [InlineData(10, 11)]
    [InlineData(1, 2)]
    public void AsDecimal_ScaleGreaterThanPrecision(byte precision, byte scale) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsDecimal(precision, scale));

    [Theory]
    [InlineData(0, 0)]
    [InlineData(39, 0)]
    public void AsDecimal_PrecisionOutOfRange(byte precision, byte scale) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsDecimal(precision, scale));
}
