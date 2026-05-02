using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

//IGNORE SPELLING: myschema

public class SqlGenerator_DeleteTests
{
    private readonly MockEncryption _mockEncrypter;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<EntityWithSchema> _sqlGeneratorForEntityWithSchema;

    public SqlGenerator_DeleteTests()
    {
        _mockEncrypter = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithoutTableAttribute = new SqlGenerator<EntityWithoutTableAttribute>(_mockEncrypter);
        _sqlGeneratorForEntityWithSchema = new SqlGenerator<EntityWithSchema>(_mockEncrypter);
    }

    [Fact]
    public void SqlDeleteString_GeneratesCorrectSql_WithTableAttribute()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Delete(testEntity);

        string expectedSql = "DELETE FROM [Test] WHERE [Id] = @Id_1;";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlDeleteString_GeneratesCorrectParameters()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Delete(testEntity);

        string expectedSql = "DELETE FROM [Test] WHERE [Id] = @Id_1;";
        Assert.Equal(expectedSql, query.QueryText);

        Assert.Single(query.Parameters);
        Assert.Equal(new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"), query.Parameters.First().Value); // Id
        Assert.Equal("@Id_1", query.Parameters.First().Key); // Id
    }

    [Fact]
    public void SqlDeleteString_ExcludesNotMappedProperties()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Id = new Guid("74e147d0-bc8b-4a22-8582-3e7b38da1695"),
            Name = "Test Name",
            Where = "Here",
            HideTimeFlag = true,
            When = "Now",
            DateOf = DateTime.UtcNow
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Delete(testEntity);

        string expectedSql = "DELETE FROM [Test] WHERE [Id] = @Id_1;";
        Assert.Equal(expectedSql, query.QueryText);

        Assert.DoesNotContain("Name", query.QueryText);
        Assert.DoesNotContain("Where", query.QueryText);
        Assert.DoesNotContain("HideTimeFlag", query.QueryText);
        Assert.DoesNotContain("When", query.QueryText);
        Assert.DoesNotContain("DateOf", query.QueryText);
        Assert.Single(query.Parameters);
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Name");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Where");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "HideTimeFlag");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "When");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "DateOf");
    }

    [Fact]
    public void SqlDeleteString_UsesClassName_WhenNoTableAttribute()
    {
        EntityWithoutTableAttribute entityWithoutTableAttribute = new()
        {
            Id = 1,
            Description = "Test Description"
        };

        SqlQuery query = _sqlGeneratorForEntityWithoutTableAttribute.Delete(entityWithoutTableAttribute);

        string expectedSql = "DELETE FROM [EntityWithoutTableAttribute] WHERE [Id] = @Id_1;";
        Assert.Equal(expectedSql, query.QueryText);
    }

    [Fact]
    public void SqlDeleteString_HandlesSchemaInTableAttribute()
    {
        EntityWithSchema entityWithSchema = new()
        {
            Id = 1,
            Description = "Test Description"
        };

        SqlQuery query = _sqlGeneratorForEntityWithSchema.Delete(entityWithSchema);

        string expectedSql = "DELETE FROM [myschema].[EntityWithSchema] WHERE [Id] = @Id_1;";
        Assert.Equal(expectedSql, query.QueryText);
    }


    [Fact]
    public void SqlDeleteString_IgnoresClassTypeProperties()
    {
        EntityWithTableAttribute testEntity = new()
        {
            Name = "Test Name",
            DateOf = new DateTime(2023, 10, 1),
            When = "Now",
            Address = new Address { Street = "123 Main St", City = "Test City", PostalCode = "37067" } // Should be ignored
        };

        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Delete(testEntity);

        string expectedSql = "DELETE FROM [Test] WHERE [Id] = @Id_1;";
        Assert.Equal(expectedSql, query.QueryText);

        Assert.DoesNotContain("Name", query.QueryText);
        Assert.DoesNotContain("DateOf", query.QueryText);
        Assert.DoesNotContain("When", query.QueryText);
        Assert.DoesNotContain("Address", query.QueryText);
        Assert.Single(query.Parameters);
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Name");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "DateOf");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "When");
        Assert.DoesNotContain(query.Parameters, param => param.Key == "Address");
    }
}