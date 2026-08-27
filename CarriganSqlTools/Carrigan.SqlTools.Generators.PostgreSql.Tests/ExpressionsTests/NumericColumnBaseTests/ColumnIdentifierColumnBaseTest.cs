using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.ColumnBaseTests;
using Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.NumericColumnBaseTests;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.NumericColumnBaseTests;

public class ColumnIdentifierColumnBaseTest : SqlServerNumericColumnBaseTest<ColumnIdentifiers>
{
    protected override string? SchemaName =>
        null;

    protected override string TableName =>
        "ColumnIdentifiers";

    protected override IEnumerable<string> NumericProperties =>
        ["Id", "Property", "ColumnName", "IdentifierName", "IdentifierOverrideName"];

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
}
