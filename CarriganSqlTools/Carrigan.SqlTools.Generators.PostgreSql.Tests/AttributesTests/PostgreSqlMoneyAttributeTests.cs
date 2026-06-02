using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.AttributesTests;

public sealed class PostgreSqlMoneyAttributeTests
{
    [Fact]
    public void Constructor()
    {
        PostgreSqlMoneyAttribute postgreSqlMoneyAttribute = new();

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(postgreSqlMoneyAttribute, "MONEY", "MONEY");
    }
}
