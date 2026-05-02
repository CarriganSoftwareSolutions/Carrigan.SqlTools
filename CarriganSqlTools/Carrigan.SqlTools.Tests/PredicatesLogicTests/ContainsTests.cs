using Carrigan.SqlTools.Dialects;
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

        string expected = "CONTAINS([ColumnTable].[Col1], @Col1_1)";
        string actual = contains.ToSqlFragments().ToSql(new SqlServerDialect());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Contains_NullColumn_ThrowsNullReferenceException() =>
    Assert.Throws<NullReferenceException>(() =>
        new Contains<ColumnTable>(null!, new Parameter("Col1", "test", null)));

    [Fact]
    public void Contains_NullParameter_ThrowsNullReferenceException() =>
        Assert.Throws<NullReferenceException>(() =>
            new Contains<ColumnTable>(new Column<ColumnTable>(nameof(ColumnTable.Col1)), null!));

}
