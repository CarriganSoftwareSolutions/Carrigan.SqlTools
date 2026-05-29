using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlVarBinaryMaxAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlVarBinaryMaxAttribute sqlVarBinaryMaxAttribute = new();

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlVarBinaryMaxAttribute,
            "VARBINARY",
            "VARBINARY(MAX)",
            expectedIsMax: true,
            expectedIsFixedLength: false);
    }
}
