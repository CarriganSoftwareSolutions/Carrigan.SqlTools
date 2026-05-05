using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Tests.Helpers;

namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;

public class AttributeExamples
{
    [Fact]
    public void AliasAttributeExample()
    {
        SelectTags tags = SelectTags.GetMany<AliasEntity>
        (
            nameof(AliasEntity.Id),
            nameof(AliasEntity.TestColumn),
            nameof(AliasEntity.NoAlias)
        );

        SqlGenerator<AliasEntity> generator = new();
        SqlQuery query = generator.Select(tags, null, null, null, null);

        string expected = "SELECT [AliasEntity].[Id], [AliasEntity].[TestColumn] AS AnAlias, [AliasEntity].[NoAlias] FROM [AliasEntity]";

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

    [Fact]
    // Yes this is the exact same test as the one above, deal with it.
    public void PrimaryKeyAttributeExample()
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