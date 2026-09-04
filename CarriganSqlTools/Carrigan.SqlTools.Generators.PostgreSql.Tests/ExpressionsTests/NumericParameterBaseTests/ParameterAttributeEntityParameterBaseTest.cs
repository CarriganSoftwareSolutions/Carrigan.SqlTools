using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.NumericParameterBaseTests;

public class ParameterAttributeEntityParameterBaseTest : PostgreSqlNumericParameterBaseTest<ParameterAttributeEntity>
{
    protected override IEnumerable<string> NumericProperties =>
        [
            nameof(ParameterAttributeEntity.Id),
            nameof(ParameterAttributeEntity.UnattributedNumeric)
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
