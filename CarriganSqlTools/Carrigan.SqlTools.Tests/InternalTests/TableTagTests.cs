using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Generators.SqlServer;

namespace Carrigan.SqlTools.Tests.InternalTests;
//IGNORE SPELLING: myschema
public class TableTagTests
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();
    [Fact]
    public void TableTag_FromClassName_NoAttributes_Test()
    {
        Assert.Equal("EntityWithoutTableAttribute", SqlGenerator<EntityWithoutTableAttribute>.Table);
        Assert.Equal("[EntityWithoutTableAttribute]", SqlGenerator<EntityWithoutTableAttribute>.Table.ToSql(Dialect));
    }
    [Fact]
    public void TableTag_FromAttributeName_NoSchemaAttributes_Test()
    {
        Assert.Equal("Test", SqlGenerator<EntityWithTableAttribute>.Table);
        Assert.Equal("[Test]", SqlGenerator<EntityWithTableAttribute>.Table.ToSql(Dialect));
    }
    [Fact]
    public void TableTag_FromAttributeName_AndSchemaAttributes_Test()
    {
        Assert.Equal("myschema.EntityWithSchema", SqlGenerator<EntityWithSchema>.Table);
        Assert.Equal("[myschema].[EntityWithSchema]", SqlGenerator<EntityWithSchema>.Table.ToSql(Dialect));
    }
}
