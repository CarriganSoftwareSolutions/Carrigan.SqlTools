using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.BooleanParameterBaseTests;

public class ParameterAttributeEntityParameterBaseTest : SqlServerBooleanParameterBaseTest<ParameterAttributeEntity>
{
    protected override IEnumerable<string> BooleanProperties =>
        [
            nameof(ParameterAttributeEntity.Enabled),
            nameof(ParameterAttributeEntity.UnattributedBoolean)
        ];

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
