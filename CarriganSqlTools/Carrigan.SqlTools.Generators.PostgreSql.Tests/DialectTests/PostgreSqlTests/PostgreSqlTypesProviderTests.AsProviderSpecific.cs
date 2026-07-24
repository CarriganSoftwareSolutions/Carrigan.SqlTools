using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsProviderSpecific_ValidProviderTypeName_ReturnsProviderSpecificType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsProviderSpecific("citext");

        AssertFieldProperties(actual, "CITEXT", isArray: null);
    }

    [Fact]
    public void AsProviderSpecific_MultiWordBuiltInType_ReturnsNormalizedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsProviderSpecific("double precision");

        AssertFieldProperties(actual, "DOUBLE PRECISION", isArray: null);
    }

    [Fact]
    public void AsProviderSpecific_NullableTrue_ReturnsNullableProviderSpecificType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsProviderSpecific("inet", true);

        AssertFieldProperties(actual, "INET", isNullable: true, isArray: null);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AsProviderSpecific_InvalidProviderTypeName_Exception(string? providerTypeName)
    {
        ArgumentException argumentException = Assert.Throws<ArgumentException>(() => PostgreSqlTypesProvider.AsProviderSpecific(providerTypeName!));
    }

    [Fact]
    public void AsProviderSpecific_InvalidProviderTypeName_NullException() =>
        Assert.Throws<ArgumentNullException>(() => PostgreSqlTypesProvider.AsProviderSpecific(null!));
    [Fact]
    public void AsProviderSpecific_SchemaQualifiedProviderTypeName_ReturnsNormalizedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsProviderSpecific("school.grade_point");

        AssertFieldProperties(actual, "SCHOOL.GRADE_POINT", isArray: null);
    }

    [Theory]
    [InlineData("INTEGER); DROP TABLE audit_log; --")]
    [InlineData("custom_type/*comment*/")]
    [InlineData("custom type")]
    [InlineData("custom_type[]")]
    public void AsProviderSpecific_SqlSyntax_ThrowsArgumentException(string providerTypeName) =>
        Assert.Throws<ArgumentException>(() => PostgreSqlTypesProvider.AsProviderSpecific(providerTypeName));

}
