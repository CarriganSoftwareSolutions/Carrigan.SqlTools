using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

public class ContainsTests
{
    [Fact]
    public void ContainsTest()
    {
        Contains<ColumnTable> contains = new(new Column<ColumnTable>(nameof(ColumnTable.Col1)), new Parameter("Col1", "test", null));

        string expected = "CONTAINS([ColumnTable].[Col1], @Parameter_Col1)";
        string actual = contains.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expected, actual);
    }
}
