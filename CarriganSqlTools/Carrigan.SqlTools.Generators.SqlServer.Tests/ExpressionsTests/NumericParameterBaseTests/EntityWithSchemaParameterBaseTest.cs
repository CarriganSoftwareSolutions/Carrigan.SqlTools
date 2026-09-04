using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.NumericParameterBaseTests;

public class EntityWithSchemaParameterBaseTest : SqlServerNumericParameterBaseTest<EntityWithSchema>
{
    protected override IEnumerable<string> NumericProperties =>
        [
            nameof(EntityWithSchema.Id)
        ];

    internal override Dictionary<string, ParameterTag> ExpectedPropertyParameterTag =>
    new
    (
        [
            NewKvp(nameof(EntityWithSchema.Id)),
            NewKvp(nameof(EntityWithSchema.Description))
        ]
    );
}
