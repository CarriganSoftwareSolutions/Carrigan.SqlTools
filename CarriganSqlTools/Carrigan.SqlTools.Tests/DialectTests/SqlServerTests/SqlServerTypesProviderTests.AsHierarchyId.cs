using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsHierarchyId_ReturnsHierarchyId() =>
        AssertSimple(SqlServerTypesProvider.AsHierarchyId(), "HIERARCHYID");

    [Fact]
    public void AsHierarchyId_ReturnsNullable_WhenNullableTrue() =>
        AssertSimple(SqlServerTypesProvider.AsHierarchyId(true), "HIERARCHYID", true);
}
