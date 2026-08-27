using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.BooleanColumnBaseTests;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.BooleanColumnBaseTests;

public class ColumnTableColumnBaseTest : SqlServerBooleanColumnBaseTest<ColumnTable>
{
    protected override string? SchemaName =>
        null;

    protected override string TableName =>
        "ColumnTable";

    protected override IEnumerable<string>BooleanProperties =>
        [];

    internal override Dictionary<string, ColumnName> ExpectedPropertyColumnName =>
        new([NewKvp("Col1"), NewKvp("Col2"), NewKvp("ColA"), NewKvp("ColB"), NewKvp("Pizza"), NewKvp("D000destruct0"), NewKvp("Express"),]);
}
