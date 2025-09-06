using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

//IGNORE SPELLING: myschema

public class SqlGenerator_SelectAllTests
{
    private readonly MockEncryption _mockEncryptor;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<EntityWithSchema> _sqlGeneratorForEntityWithSchema;

    public SqlGenerator_SelectAllTests()
    {
        _mockEncryptor = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(_mockEncryptor);
        _sqlGeneratorForEntityWithoutTableAttribute = new SqlGenerator<EntityWithoutTableAttribute>(_mockEncryptor);
        _sqlGeneratorForEntityWithSchema = new SqlGenerator<EntityWithSchema>(_mockEncryptor);
    }

    [Fact]
    public void SqlSelect_GeneratesCorrectSql_WithTableAttribute()
    {
        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.SelectAll();

        string expectedSql = "SELECT [Test].* FROM [Test]";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_UsesClassName_WhenNoTableAttribute()
    {
        SqlQuery query = _sqlGeneratorForEntityWithoutTableAttribute.SelectAll();

        string expectedSql = "SELECT [EntityWithoutTableAttribute].* FROM [EntityWithoutTableAttribute]";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_HandlesSchemaInTableAttribute()
    {
        SqlQuery query = _sqlGeneratorForEntityWithSchema.SelectAll();

        string expectedSql = "SELECT [myschema].[EntityWithSchema].* FROM [myschema].[EntityWithSchema]";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlSelect_HandlesSchemaInTableAttribute_WithOrderBy()
    {
        OrderByItem<EntityWithSchema> orderByItems = new(nameof(EntityWithSchema.Id));
        SqlQuery query = _sqlGeneratorForEntityWithSchema.SelectAll(new(orderByItems));

        string expectedSql = "SELECT [myschema].[EntityWithSchema].* FROM [myschema].[EntityWithSchema] ORDER BY [myschema].[EntityWithSchema].[Id] ASC";
        Assert.Equal(expectedSql, query.QueryText);
    }
}
