using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.ColumnBaseTests;

public class EntityWithTableAttributeColumnBaseTest : PostgreSqlColumnBaseTest<EntityWithTableAttribute>
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

    protected override ColumnBase NewColumn(string propertyName) =>
        new Column<EntityWithTableAttribute>(propertyName);
    protected override ColumnBase NewColumn(PropertyName propertyName) =>
        new Column<EntityWithTableAttribute>(propertyName);

    protected override SqlExpression NewColumnAsExpression(string propertyName) =>
        NewColumn(propertyName);
    protected override SqlExpression NewColumnAsExpression(PropertyName propertyName) =>
        NewColumn(propertyName);
}
