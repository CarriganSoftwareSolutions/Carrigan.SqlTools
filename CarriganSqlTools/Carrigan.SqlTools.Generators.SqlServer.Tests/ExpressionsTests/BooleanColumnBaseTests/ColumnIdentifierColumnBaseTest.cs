using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.ColumnBaseTests;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.BooleanColumnBaseTests;

public class ColumnIdentifierColumnBaseTest : SqlServerBooleanColumnBaseTest<ColumnIdentifiers>
{
    protected override string? SchemaName =>
        null;

    protected override string TableName =>
        "ColumnIdentifiers";

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
}
