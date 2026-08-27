using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.ColumnBaseTests;

public class ColumnTableColumnBaseTest : SqlServerColumnBaseTest<ColumnTable>
{
    protected override string? SchemaName =>
        null;

    protected override string TableName =>
        "ColumnTable";

    protected override IEnumerable<string> NumericProperties =>
        [];

    protected override IEnumerable<string> BooleanProperties =>
        [];

    internal override Dictionary<string, ColumnName> ExpectedPropertyColumnName =>
        new([NewKvp("Col1"), NewKvp("Col2"), NewKvp("ColA"), NewKvp("ColB"), NewKvp("Pizza"), NewKvp("D000destruct0"), NewKvp("Express"),]);

    protected override ColumnBase NewColumn(string propertyName) =>
        new Column<ColumnTable>(propertyName);
    protected override ColumnBase NewColumn(PropertyName propertyName) =>
        new Column<ColumnTable>(propertyName);
}
