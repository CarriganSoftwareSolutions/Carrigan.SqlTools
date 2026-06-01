using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

internal static class SqlTypeAttributeTestHelpers
{
    private static readonly SqlServerDialect Dialect = new();

    internal static void AssertFieldProperties(
        SqlTypeAttribute attribute,
        string expectedProviderTypeName,
        string expectedTypeDeclaration,
        int? expectedLength = null,
        bool? expectedIsMax = null,
        bool? expectedIsUnicode = null,
        bool? expectedIsFixedLength = null,
        byte? expectedPrecision = null,
        byte? expectedScale = null,
        byte? expectedFractionalSecondsPrecision = null)
    {
        Assert.NotNull(attribute);
        Assert.NotNull(attribute.FieldProperties);

        FieldProperties fieldProperties = attribute.FieldProperties;

        Assert.Equal(expectedProviderTypeName, fieldProperties.ProviderTypeName);
        Assert.Equal(expectedLength, fieldProperties.Length);
        Assert.Equal(expectedIsMax, fieldProperties.IsMax);
        Assert.Equal(expectedIsUnicode, fieldProperties.IsUnicode);
        Assert.Equal(expectedIsFixedLength, fieldProperties.IsFixedLength);
        Assert.Equal(expectedPrecision, fieldProperties.Precision);
        Assert.Equal(expectedScale, fieldProperties.Scale);
        Assert.Equal(expectedFractionalSecondsPrecision, fieldProperties.FractionalSecondsPrecision);
        Assert.False(fieldProperties.IsNullable);
        Assert.Equal($"{expectedTypeDeclaration} NOT NULL", Dialect.RenderFieldProperties(fieldProperties));
    }
}
