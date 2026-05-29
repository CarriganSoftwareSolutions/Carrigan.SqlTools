using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class ContainsTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void ContainsTest()
    {
        Contains<ColumnTable> contains = new(new Column<ColumnTable>(nameof(ColumnTable.Col1)), new Parameter("test", "Col1"));

        string expected = "CONTAINS([ColumnTable].[Col1], @Col1_1)";
        string actual = contains.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Contains_NullColumn_ThrowsNullReferenceException() =>
    Assert.Throws<NullReferenceException>(() =>
        new Contains<ColumnTable>(null!, new Parameter("test", "Col1")));

    [Fact]
    public void Contains_NullParameter_ThrowsNullReferenceException() =>
        Assert.Throws<NullReferenceException>(() =>
            new Contains<ColumnTable>(new Column<ColumnTable>(nameof(ColumnTable.Col1)), null!));

}
