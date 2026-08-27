using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.NumericColumnBaseTests;

public class EntityWithTableAttributeColumnBaseTest : SqlServerNumericColumnBaseTest<EntityWithTableAttribute>
{
    protected override string? SchemaName =>
        null;

    protected override string TableName =>
        "Test";

    protected override IEnumerable<string> NumericProperties =>
        [];

    internal override Dictionary<string, ColumnName> ExpectedPropertyColumnName =>
        new([NewKvp("Id"), NewKvp("Name"), NewKvp("DateOf"), NewKvp("When")]);

    protected override IEnumerable<string> NotMappedProperties =>
        ["Where", "HideTimeFlag"];
}
