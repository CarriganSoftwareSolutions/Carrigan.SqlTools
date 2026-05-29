using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlDateAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlDateAttribute sqlDateAttribute = new();

        SqlTypeAttributeTestHelpers.AssertFieldProperties(sqlDateAttribute, "DATE", "DATE");
    }
}
