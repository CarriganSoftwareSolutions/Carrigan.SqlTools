using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GeneratorsTests;

public sealed class SqlGenerator_DeleteArgumentValidationTests
{
    [Fact]
    public void Delete_NullEntity_Exception()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new();
        Assert.Throws<ArgumentNullException>(() => generator.Delete((EntityWithTableAttribute)null!));
    }

    [Fact]
    public void DeleteById_NullEntities_Exception()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new();
        Assert.Throws<ArgumentNullException>(() => generator.DeleteById(null!));
    }
}
