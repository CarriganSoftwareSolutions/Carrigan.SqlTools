using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public class SqlGenerator_DeleteAllTests
{
    private readonly MockEncryption _mockEncryptor;
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;
    private readonly SqlGenerator<EntityWithoutTableAttribute> _sqlGeneratorForEntityWithoutTableAttribute;
    private readonly SqlGenerator<EntityWithSchema> _sqlGeneratorForEntityWithSchema;

    public SqlGenerator_DeleteAllTests()
    {
        _mockEncryptor = new MockEncryption("+Encrypted+");
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(_mockEncryptor);
        _sqlGeneratorForEntityWithoutTableAttribute = new SqlGenerator<EntityWithoutTableAttribute>(_mockEncryptor);
        _sqlGeneratorForEntityWithSchema = new SqlGenerator<EntityWithSchema>(_mockEncryptor);
    }

    [Fact]
    public void SqlDeleteAll_GeneratesCorrectSql_WithTableAttribute()
    {
        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.DeleteAll();

        string expectedSql = "DELETE FROM [Test];";
        Assert.Equal(expectedSql, query.QueryText);
    }
}
