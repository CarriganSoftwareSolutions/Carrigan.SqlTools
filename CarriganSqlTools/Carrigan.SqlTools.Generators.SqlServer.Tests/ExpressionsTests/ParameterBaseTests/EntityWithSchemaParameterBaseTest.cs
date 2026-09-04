using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.ParameterBaseTests;

public class EntityWithSchemaParameterBaseTest : SqlServerParameterBaseTest<EntityWithSchema>
{
    internal override Dictionary<string, ParameterTag> ExpectedPropertyParameterTag =>
    new
    (
        [
            NewKvp(nameof(EntityWithSchema.Id)),
            NewKvp(nameof(EntityWithSchema.Description))
        ]
    );
}
