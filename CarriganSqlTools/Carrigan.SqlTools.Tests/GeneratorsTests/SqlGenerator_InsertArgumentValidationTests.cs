using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public sealed class SqlGenerator_InsertArgumentValidationTests
{
    [Fact]
    public void Insert_EntitiesNull_ArgumentException()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new(new SqlServerDialect());

        IEnumerable<EntityWithTableAttribute> entities = null!;

        Assert.Throws<ArgumentException>(() => generator.Insert(null, null, entities));
    }

    [Fact]
    public void Insert_EntitiesEmpty_ArgumentException()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new(new SqlServerDialect());

        Assert.Throws<ArgumentException>(() => generator.Insert(null, null, []));
    }
}
