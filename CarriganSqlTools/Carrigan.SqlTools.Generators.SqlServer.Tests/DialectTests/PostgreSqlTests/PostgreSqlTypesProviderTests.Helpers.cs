using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    private static void AssertFieldProperties(
        FieldProperties actual,
        string providerTypeName,
        int? length = null,
        bool? isMax = null,
        bool? isUnicode = null,
        bool? isFixedLength = null,
        byte? precision = null,
        byte? scale = null,
        byte? fractionalSecondsPrecision = null,
        string? baseType = null,
        bool isNullable = false)
    {
        Assert.Equal(providerTypeName, actual.ProviderTypeName);
        Assert.Equal(length, actual.Length);
        Assert.Equal(isMax, actual.IsMax);
        Assert.Equal(isUnicode, actual.IsUnicode);
        Assert.Equal(isFixedLength, actual.IsFixedLength);
        Assert.Equal(precision, actual.Precision);
        Assert.Equal(scale, actual.Scale);
        Assert.Equal(fractionalSecondsPrecision, actual.FractionalSecondsPrecision);
        Assert.Equal(baseType, actual.BaseType);
        Assert.Equal(isNullable, actual.IsNullable);
    }
}
