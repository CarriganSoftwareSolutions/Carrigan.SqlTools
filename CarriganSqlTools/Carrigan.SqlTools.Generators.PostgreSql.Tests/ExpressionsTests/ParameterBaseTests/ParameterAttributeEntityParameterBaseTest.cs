using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.ParameterBaseTests;

public class ParameterAttributeEntityParameterBaseTest : PostgreSqlParameterBaseTest<ParameterAttributeEntity>
{
    internal override Dictionary<string, ParameterTag> ExpectedPropertyParameterTag =>
    new
    (
        [
            NewKvp(nameof(ParameterAttributeEntity.Id), "IdParameter"),
            NewKvp(nameof(ParameterAttributeEntity.Description), "DescriptionParameter"),
            NewKvp(nameof(ParameterAttributeEntity.Enabled), "EnabledParameter"),
            NewKvp(nameof(ParameterAttributeEntity.UnattributedNumeric)),
            NewKvp(nameof(ParameterAttributeEntity.UnattributedBoolean))
        ]
    );
}
