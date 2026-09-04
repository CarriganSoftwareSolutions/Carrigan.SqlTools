using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.NumericParameterBaseTests;

public class EntityWithSchemaParameterBaseTest : PostgreSqlNumericParameterBaseTest<EntityWithSchema>
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
