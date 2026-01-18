using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities;
using System.Data;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public sealed class SqlGenerator_ProcedureArgumentValidationTests
{
    [Fact]
    public void Procedure_NullEntity_ArgumentNullException()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new();

        Assert.Throws<ArgumentNullException>(() => generator.Procedure(null!));
    }

    [Fact]
    public void Procedure_SetsCommandTypeStoredProcedure()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new();

        EntityWithTableAttribute entity = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Name",
            When = "Now",
            DateOf = new DateTime(2023, 10, 1)
        };

        SqlQuery query = generator.Procedure(entity);

        Assert.Equal(CommandType.StoredProcedure, query.CommandType);
    }
}
