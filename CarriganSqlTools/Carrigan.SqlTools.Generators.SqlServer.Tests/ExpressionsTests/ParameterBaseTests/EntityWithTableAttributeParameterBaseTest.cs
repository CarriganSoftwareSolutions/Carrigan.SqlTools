using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.ParameterBaseTests;

public class EntityWithTableAttributeParameterBaseTest : SqlServerParameterBaseTest<EntityWithTableAttribute>
{
    internal override Dictionary<string, ParameterTag> ExpectedPropertyParameterTag =>
    new
    (
        [
            NewKvp(nameof(EntityWithTableAttribute.Id)),
            NewKvp(nameof(EntityWithTableAttribute.Name)),
            NewKvp(nameof(EntityWithTableAttribute.DateOf)),
            NewKvp(nameof(EntityWithTableAttribute.When))
        ]
    );

    protected override IEnumerable<string> NotMappedProperties =>
        [
            nameof(EntityWithTableAttribute.Where),
            nameof(EntityWithTableAttribute.HideTimeFlag)
        ];
}
