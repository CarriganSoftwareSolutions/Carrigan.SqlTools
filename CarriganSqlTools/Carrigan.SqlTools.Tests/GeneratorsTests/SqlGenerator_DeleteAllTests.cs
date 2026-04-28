using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public class SqlGenerator_DeleteAllTests
{
    private readonly SqlGenerator<EntityWithTableAttribute> _sqlGeneratorForEntityWithTableAttribute;

    public SqlGenerator_DeleteAllTests() => 
        _sqlGeneratorForEntityWithTableAttribute = new SqlGenerator<EntityWithTableAttribute>(new SqlServerDialect());

    [Fact]
    public void SqlDeleteAll_GeneratesCorrectSql_WithTableAttribute()
    {
        SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.DeleteAll();

        string expectedSql = "DELETE FROM [Test];";
        Assert.Equal(expectedSql, query.QueryText);
    }
}
