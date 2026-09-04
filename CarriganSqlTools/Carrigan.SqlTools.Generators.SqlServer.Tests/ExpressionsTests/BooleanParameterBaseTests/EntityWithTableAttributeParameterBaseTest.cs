using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.BooleanParameterBaseTests;

public class EntityWithTableAttributeParameterBaseTest : SqlServerBooleanParameterBaseTest<EntityWithTableAttribute>
{
    protected override IEnumerable<string> BooleanProperties =>
        [];

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
