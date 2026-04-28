using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public sealed class SqlGenerator_DeleteArgumentValidationTests
{
    [Fact]
    public void Delete_NullEntity_Exception()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new(new SqlServerDialect());
        Assert.Throws<ArgumentNullException>(() => generator.Delete(null!));
    }

    [Fact]
    public void DeleteById_NullEntities_Exception()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new(new SqlServerDialect());
        Assert.Throws<ArgumentNullException>(() => generator.DeleteById(null!));
    }
}
