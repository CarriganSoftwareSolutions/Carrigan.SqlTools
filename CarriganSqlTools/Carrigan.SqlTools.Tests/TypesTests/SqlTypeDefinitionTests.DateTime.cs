using System.Data;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.TypesTests;

public sealed partial class SqlTypeDefinitionTests
{
    [Theory]
    [InlineData(null, "DATETIME2")]
    [InlineData((byte)0, "DATETIME2(0)")]
    [InlineData((byte)7, "DATETIME2(7)")]
    public void AsDateTime2(byte? fractionalSecondPrecision, string expectedDeclaration)
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsDateTime2(fractionalSecondPrecision);
        Assert.Equal(SqlDbType.DateTime2, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
        Assert.Equal(fractionalSecondPrecision, definition.Scale);
    }

    [Theory]
    [InlineData((byte)8)]
    [InlineData((byte)255)]
    public void AsDateTime2_OutOfRange(byte fractionalSecondPrecision) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsDateTime2(fractionalSecondPrecision));

    [Theory]
    [InlineData(null, "DATETIMEOFFSET")]
    [InlineData((byte)0, "DATETIMEOFFSET(0)")]
    [InlineData((byte)7, "DATETIMEOFFSET(7)")]
    public void AsDateTimeOffset(byte? fractionalSecondPrecision, string expectedDeclaration)
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsDateTimeOffset(fractionalSecondPrecision);
        Assert.Equal(SqlDbType.DateTimeOffset, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
        Assert.Equal(fractionalSecondPrecision, definition.Scale);
    }

    [Theory]
    [InlineData((byte)8)]
    [InlineData((byte)200)]
    public void AsDateTimeOffset_OutOfRange(byte fractionalSecondPrecision) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsDateTimeOffset(fractionalSecondPrecision));

    [Theory]
    [InlineData(null, "TIME")]
    [InlineData((byte)0, "TIME(0)")]
    [InlineData((byte)7, "TIME(7)")]
    public void AsTime(byte? fractionalSecondPrecision, string expectedDeclaration)
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsTime(fractionalSecondPrecision);
        Assert.Equal(SqlDbType.Time, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
        Assert.Equal(fractionalSecondPrecision, definition.Scale);
    }

    [Theory]
    [InlineData((byte)8)]
    [InlineData((byte)128)]
    public void AsTime_OutOfRange(byte fractionalSecondPrecision) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsTime(fractionalSecondPrecision));
}
