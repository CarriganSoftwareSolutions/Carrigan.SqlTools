using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Base.Tests.TestEntities;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GeneratorsTests;

public sealed class SqlGenerator_InsertArgumentValidationTests
{
    [Fact]
    public void Insert_EntitiesNull_ArgumentException()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new();

        IEnumerable<EntityWithTableAttribute> entities = null!;

        Assert.Throws<ArgumentException>(() => generator.Insert(null, null, entities));
    }

    [Fact]
    public void Insert_EntitiesEmpty_ArgumentException()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new();

        Assert.Throws<ArgumentException>(() => generator.Insert(null, null, []));
    }
}
