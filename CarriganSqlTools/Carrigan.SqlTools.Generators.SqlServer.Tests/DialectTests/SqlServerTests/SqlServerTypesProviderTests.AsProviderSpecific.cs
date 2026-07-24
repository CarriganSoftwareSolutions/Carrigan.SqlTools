using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsProviderSpecific_ReturnsNormalizedProviderTypeName() => 
        AssertProviderSpecific(SqlServerTypesProvider.AsProviderSpecific("custom_type"), "CUSTOM_TYPE");

    [Fact]
    public void AsProviderSpecific_ReturnsNullable_WhenNullableTrue() =>
        AssertProviderSpecific(SqlServerTypesProvider.AsProviderSpecific("custom_type", true), "CUSTOM_TYPE", true);

    [Fact]
    public void AsProviderSpecific_Throws_WhenProviderTypeNameIsNull() =>
        Assert.Throws<ArgumentNullException>(() => SqlServerTypesProvider.AsProviderSpecific(null!));

    [Fact]
    public void AsProviderSpecific_Throws_WhenProviderTypeNameIsWhiteSpace() => 
        Assert.Throws<ArgumentException>(() => SqlServerTypesProvider.AsProviderSpecific(" "));
    [Fact]
    public void AsProviderSpecific_ReturnsNormalizedSchemaQualifiedProviderTypeName() =>
        AssertProviderSpecific(SqlServerTypesProvider.AsProviderSpecific("school.grade_point"), "SCHOOL.GRADE_POINT");

    [Theory]
    [InlineData("INT); DROP TABLE AuditLog; --")]
    [InlineData("custom_type/*comment*/")]
    [InlineData("custom type")]
    public void AsProviderSpecific_Throws_WhenProviderTypeNameContainsSqlSyntax(string providerTypeName) =>
        Assert.Throws<ArgumentException>(() => SqlServerTypesProvider.AsProviderSpecific(providerTypeName));

}
