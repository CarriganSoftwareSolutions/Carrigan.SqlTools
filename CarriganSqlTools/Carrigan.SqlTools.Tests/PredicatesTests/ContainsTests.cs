using SqlTools.Predicates;
using SqlToolsTests.TestEntities;

namespace SqlToolsTests.PredicatesTests;

public class ContainsTests
{
    [Fact]
    public void ContainsTest()
    {
        Contains<ColumnTable> contains = new(new Columns<ColumnTable>(nameof(ColumnTable.Col1)), new Parameters("Col1", "test"));

        string expected = "CONTAINS([ColumnTable].[Col1], @Parameter_Col1)";
        string actual = contains.ToSql();

        Assert.Equal(expected, actual);
    }
}
