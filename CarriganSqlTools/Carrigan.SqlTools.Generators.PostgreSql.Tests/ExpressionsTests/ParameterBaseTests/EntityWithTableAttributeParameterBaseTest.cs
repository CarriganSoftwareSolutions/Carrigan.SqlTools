using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.ParameterBaseTests;

public class EntityWithTableAttributeParameterBaseTest : PostgreSqlParameterBaseTest<EntityWithTableAttribute>
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
