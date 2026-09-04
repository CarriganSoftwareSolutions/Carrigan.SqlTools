using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.BooleanParameterBaseTests;

public class EntityWithSchemaParameterBaseTest : PostgreSqlBooleanParameterBaseTest<EntityWithSchema>
{
    protected override IEnumerable<string> BooleanProperties =>
        [];

    internal override Dictionary<string, ParameterTag> ExpectedPropertyParameterTag =>
    new
    (
        [
            NewKvp(nameof(EntityWithSchema.Id)),
            NewKvp(nameof(EntityWithSchema.Description))
        ]
    );
}
