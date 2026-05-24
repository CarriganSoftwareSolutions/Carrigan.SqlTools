using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GeneratorsTests;

//IGNORE SPELLING: myschema

public class SqlGenerator_SelectAllTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<EntityWithSchema> _sqlGeneratorForEntityWithSchema;

    public SqlGenerator_SelectAllTests()
    {
        _mockEncrypter = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithoutTableAttribute = new SqlGenerator<EntityWithoutTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithSchema = new SqlGenerator<EntityWithSchema>(_mockEncrypter);
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
        OrderBy<EntityWithSchema> orderByItems = new(nameof(EntityWithSchema.Id));
        SqlQuery query = _sqlGeneratorForEntityWithSchema.SelectAll(new OrderBys(orderByItems));

        string expectedSql = "SELECT [myschema].[EntityWithSchema].* FROM [myschema].[EntityWithSchema] ORDER BY [myschema].[EntityWithSchema].[Id] ASC";
        Assert.Equal(expectedSql, query.QueryText);
    }
}
