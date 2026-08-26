using Carrigan.SqlTools.Base.Tests;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests.ColumnBaseTests;

public class ColumnIdentifierColumnBaseTest : SqlServerColumnBaseTest<ColumnIdentifiers>
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

    protected override ColumnBase NewColumn(string propertyName) =>
        new Column<ColumnIdentifiers>(propertyName);
    protected override ColumnBase NewColumn(PropertyName propertyName) =>
        new Column<ColumnIdentifiers>(propertyName);
}
