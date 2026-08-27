using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.BooleanColumnBaseTests;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.BooleanColumnBaseTests;

public class EntityWithTableAttributeColumnBaseTest : SqlServerBooleanColumnBaseTest<EntityWithTableAttribute>
{
    protected override string? SchemaName =>
        null;

    protected override string TableName =>
        "Test";

    protected override IEnumerable<string> BooleanProperties =>
        [];

    internal override Dictionary<string, ColumnName> ExpectedPropertyColumnName =>
        new([NewKvp("Id"), NewKvp("Name"), NewKvp("DateOf"), NewKvp("When")]);

    protected override IEnumerable<string> NotMappedProperties =>
        ["Where", "HideTimeFlag"];
}
