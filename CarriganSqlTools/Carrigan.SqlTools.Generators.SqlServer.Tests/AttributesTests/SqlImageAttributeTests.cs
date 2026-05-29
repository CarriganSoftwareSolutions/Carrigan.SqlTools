using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlImageAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlImageAttribute sqlImageAttribute = new();

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlImageAttribute,
            "IMAGE",
            "IMAGE",
            expectedIsMax: false,
            expectedIsFixedLength: false);
    }
}
