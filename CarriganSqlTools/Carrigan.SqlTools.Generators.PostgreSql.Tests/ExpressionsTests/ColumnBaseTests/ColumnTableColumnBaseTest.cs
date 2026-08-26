using Carrigan.SqlTools.Base.Tests;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.ColumnBaseTests;

public class ColumnTableColumnBaseTest : PostgreSqlColumnBaseTest<ColumnTable>
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
