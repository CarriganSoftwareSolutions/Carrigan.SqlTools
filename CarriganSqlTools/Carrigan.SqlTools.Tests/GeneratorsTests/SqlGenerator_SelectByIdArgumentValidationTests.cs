using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public sealed class SqlGenerator_SelectByIdArgumentValidationTests
{
    [Fact]
    public void SelectById_EntitiesNull_Exception()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new(new SqlServerDialect());
        Assert.Throws<ArgumentNullException>(() => generator.SelectById(null!));
    }

    [Fact]
    public void SelectById_EntitiesEmpty_Exception()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new(new SqlServerDialect());
        Assert.Throws<ArgumentException>(() => generator.SelectById([]));
    }
}
