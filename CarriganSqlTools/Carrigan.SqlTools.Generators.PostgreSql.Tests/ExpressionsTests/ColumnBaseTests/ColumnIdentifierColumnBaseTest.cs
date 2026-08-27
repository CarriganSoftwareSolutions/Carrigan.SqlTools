using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.ColumnBaseTests;

public class ColumnIdentifierColumnBaseTest : PostgreSqlColumnBaseTest<ColumnIdentifiers>
{
    protected override string? SchemaName =>
        null;

    protected override string TableName =>
        "ColumnIdentifiers";

    protected override IEnumerable<string> NumericProperties =>
        ["Id", "Property", "ColumnName", "IdentifierName", "IdentifierOverrideName"];

    protected override IEnumerable<string> BooleanProperties =>
        [];

    internal override Dictionary<string, ColumnName> ExpectedPropertyColumnName =>
    new
    (
        [
            NewKvp("Id"), 
            NewKvp("Property"), 
            NewKvp("ColumnName", "Column"), 
            NewKvp("IdentifierName", "Identifier"), 
            NewKvp("IdentifierOverrideName", "IdentifierOverride")
        ]
    );

    protected override ColumnBase NewColumn(string propertyName) =>
        new Column<ColumnIdentifiers>(propertyName);
    protected override ColumnBase NewColumn(PropertyName propertyName) =>
        new Column<ColumnIdentifiers>(propertyName);
}
