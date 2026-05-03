using System;
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

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
}
