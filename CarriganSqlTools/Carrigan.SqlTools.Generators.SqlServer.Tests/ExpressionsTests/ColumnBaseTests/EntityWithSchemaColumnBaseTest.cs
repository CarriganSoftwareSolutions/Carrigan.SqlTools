using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.ColumnBaseTests;

public class EntityWithSchemaColumnBaseTest : SqlServerColumnBaseTest<EntityWithSchema>
{
    protected override string? SchemaName =>
        "myschema";

    protected override string TableName =>
        "EntityWithSchema";

    protected override IEnumerable<string> NumericProperties =>
        ["Id"];

    protected override IEnumerable<string> BooleanProperties =>
        [];

    internal override Dictionary<string, ColumnName> ExpectedPropertyColumnName =>
        new([NewKvp("Id"), NewKvp("Description")]);

    protected override ColumnBase NewColumn(string propertyName) =>
        new Column<EntityWithSchema>(propertyName);
    protected override ColumnBase NewColumn(PropertyName propertyName) =>
        new Column<EntityWithSchema>(propertyName);
}
