using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.AttributesTests;

public sealed class PostgreSqlDateAttributeTests
{
    [Fact]
    public void Constructor()
    {
        PostgreSqlDateAttribute postgreSqlDateAttribute = new();

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(postgreSqlDateAttribute, "DATE", "DATE");
    }
}
