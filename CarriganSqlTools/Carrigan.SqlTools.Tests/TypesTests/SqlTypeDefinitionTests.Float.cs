using System.Data;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.TypesTests;

public sealed partial class SqlTypeDefinitionTests
{
    [Theory]
    [InlineData(null, "FLOAT")]
    [InlineData((byte)1, "FLOAT(1)")]
    [InlineData((byte)53, "FLOAT(53)")]
    public void AsFloat_WithOptionalPrecision_GeneratesExpected(byte? precision, string expectedDeclaration)
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsFloat(precision);
        Assert.Equal(SqlDbType.Float, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
        Assert.Equal(precision, definition.Precision);
    }

    [Theory]
    [InlineData((byte)0)]
    [InlineData((byte)54)]
    public void AsFloat_PrecisionOutOfRange_Throws(byte precision) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsFloat(precision));
}
