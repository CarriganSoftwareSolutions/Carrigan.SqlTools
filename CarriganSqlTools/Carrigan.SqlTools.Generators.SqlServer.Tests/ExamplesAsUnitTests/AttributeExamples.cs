using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

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
        SqlQuery query = generator.Select(null, null, tags, null, null, null, null);

        string expected = "SELECT [AliasEntity].[Id], [AliasEntity].[TestColumn] AS [AnAlias], [AliasEntity].[NoAlias] FROM [AliasEntity]";

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

        string expectedSql = "UPDATE [schema].[Email] SET [CustomerId] = @CustomerId_1, [Email] = @Email_2 WHERE [Id] = @Id_3;";
        string actualSql = query.QueryText;
        Assert.Equal(expectedSql, actualSql);
        SqlQueryTestHelper.AssertParameterCount(query, 3);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_3", 10);
        SqlQueryTestHelper.AssertParameterValue(query, "@CustomerId_1", 313);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "Exterminate@GenericTinCanLand.gov");
    }
}