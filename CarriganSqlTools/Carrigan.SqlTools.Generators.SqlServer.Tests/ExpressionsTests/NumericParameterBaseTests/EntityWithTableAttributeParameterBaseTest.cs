using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.NumericParameterBaseTests;

public class EntityWithTableAttributeParameterBaseTest : SqlServerNumericParameterBaseTest<EntityWithTableAttribute>
{
    protected override IEnumerable<string> NumericProperties =>
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
