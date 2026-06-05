using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class AttributeExamples
{
    [Fact]
    public void AliasAttributeExample()
    {
        SelectTags tags = SelectTagGenerator.GetMany<AliasEntity>
        (
            nameof(AliasEntity.Id),
            nameof(AliasEntity.TestColumn),
            nameof(AliasEntity.NoAlias)
        );

        SqlGenerator<AliasEntity> generator = new();
        SelectBuilder<AliasEntity> selectBuilder = new()
        {
            Selects = tags
        };

        SqlQuery query = generator.Select(selectBuilder);

        string expected =
            """
            SELECT "AliasEntity"."Id", "AliasEntity"."TestColumn" AS "AnAlias", "AliasEntity"."NoAlias" FROM "AliasEntity"
            """;

        Assert.Equal(expected, query.QueryText);
    }

    [Fact]
    public void IdentifierAttributeExample()
    {
        SqlGenerator<EmailModel> emailGenerator = new();
        EmailModel email = new()
        {
            Id = 10,
            CustomerId = 313,
            EmailAddress = "Exterminate@GenericTinCanLand.gov"
        };
        SqlQuery query = emailGenerator.UpdateById(email);

        string expectedSql = """UPDATE "schema"."Email" SET "CustomerId" = $1, "Email" = $2 WHERE "Id" = $3;""";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);
        SqlQueryTestHelper.AssertParameterCount(query, 3);
        SqlQueryTestHelper.AssertParameterValue(query, "$3", 10);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 313);
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "Exterminate@GenericTinCanLand.gov");
    }
}