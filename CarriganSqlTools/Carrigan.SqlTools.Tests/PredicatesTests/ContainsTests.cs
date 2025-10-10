using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

public class ContainsTests
{
    [Fact]
    public void ContainsTest()
    {
        Contains<ColumnTable> contains = new(new Column<ColumnTable>(nameof(ColumnTable.Col1)), new Parameter("Col1", "test"));

        string expected = "CONTAINS([ColumnTable].[Col1], @Parameter_Col1)";
        string actual = contains.ToSql();

        Assert.Equal(expected, actual);
    }
}
