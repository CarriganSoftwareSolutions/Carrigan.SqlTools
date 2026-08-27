using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.NumericColumnBaseTests;

public class ColumnTableColumnBaseTest : SqlServerNumericColumnBaseTest<ColumnTable>
{
    protected override string? SchemaName =>
        null;

    protected override string TableName =>
        "ColumnTable";

    protected override IEnumerable<string> NumericProperties =>
        [];

    internal override Dictionary<string, ColumnName> ExpectedPropertyColumnName =>
        new([NewKvp("Col1"), NewKvp("Col2"), NewKvp("ColA"), NewKvp("ColB"), NewKvp("Pizza"), NewKvp("D000destruct0"), NewKvp("Express"),]);
}
