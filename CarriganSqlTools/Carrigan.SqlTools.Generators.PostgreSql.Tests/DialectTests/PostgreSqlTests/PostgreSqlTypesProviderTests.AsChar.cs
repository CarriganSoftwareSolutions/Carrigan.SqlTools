using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsChar_ValidLength_ReturnsChar()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsChar(10);

        AssertFieldProperties(actual, "CHAR", length: 10, isUnicode: true, isFixedLength: true);
    }
    [Fact]
    public void AsChar_NullLength_ValidLength_ReturnsChar()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsChar(null);

        AssertFieldProperties(actual, "CHAR", length: null, isUnicode: true, isFixedLength: true);
    }

    [Fact]
    public void AsChar_NullableTrue_ReturnsNullableChar()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsChar(10, true);

        AssertFieldProperties(actual, "CHAR", length: 10, isUnicode: true, isFixedLength: true, isNullable: true);
    }

    [Fact]
    public void AsChar_NullLength_NullableTrue_ReturnsNullableChar()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsChar(null, true);

        AssertFieldProperties(actual, "CHAR", length: null, isUnicode: true, isFixedLength: true, isNullable: true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10_485_761)]
    public void AsChar_InvalidLength_Exception(int length) => 
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsChar(length));
}
