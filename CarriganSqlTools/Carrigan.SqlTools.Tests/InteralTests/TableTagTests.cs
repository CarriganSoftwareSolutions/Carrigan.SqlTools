using SqlTools.SqlGenerators;
using SqlToolsTests.TestEntities;

namespace SqlToolsTests.InteralTests;
//IGNORE SPELLING: myschema
public class TableTagTests
{

    [Fact]
    public void TableTag_FromClassName_NoAttributes_Test()
    {
        string actualValue = SqlGenerator<EntityWithoutTableAttribute>.TableTag;
        string expectedValue = "[EntityWithoutTableAttribute]";

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void TableTag_FromAttributeName_NoSchemaAttributes_Test()
    {
        string actualValue = SqlGenerator<EntityWithTableAttribute>.TableTag;
        string expectedValue = "[Test]";

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void TableTag_FromAttributeName_AndSchemaAttributes_Test()
    {
        string actualValue = SqlGenerator<EntityWithSchema>.TableTag;
        string expectedValue = "[myschema].[EntityWithSchema]";

        Assert.Equal(expectedValue, actualValue);
    }
}
