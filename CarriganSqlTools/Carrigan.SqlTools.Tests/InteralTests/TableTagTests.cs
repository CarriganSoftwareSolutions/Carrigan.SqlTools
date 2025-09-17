using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.InteralTests;
//IGNORE SPELLING: myschema
public class TableTagTests
{

    [Fact]
    public void TableTag_FromClassName_NoAttributes_Test()
    {
        string actualValue = SqlGenerator<EntityWithoutTableAttribute>.Table;
        string expectedValue = "[EntityWithoutTableAttribute]";

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void TableTag_FromAttributeName_NoSchemaAttributes_Test()
    {
        string actualValue = SqlGenerator<EntityWithTableAttribute>.Table;
        string expectedValue = "[Test]";

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void TableTag_FromAttributeName_AndSchemaAttributes_Test()
    {
        string actualValue = SqlGenerator<EntityWithSchema>.Table;
        string expectedValue = "[myschema].[EntityWithSchema]";

        Assert.Equal(expectedValue, actualValue);
    }
}
