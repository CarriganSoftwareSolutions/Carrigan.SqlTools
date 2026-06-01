using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class AndTests
{
    private static readonly SqlServerDialect Dialect = new ();

    [Fact]
    public void And_Empty_ToSql() =>
        Assert.Throws<ArgumentException>(() => new And([]));

    [Fact]
    public void And_null_ToSql() =>
        Assert.Throws<ArgumentNullException>(() => new And(null!));

    [Fact]
    public void And_Single_ToSql()
    {
        And and = new(
        [
            new Parameter(1, "P1"),
        ]);

        string expected = $"@P1_1";
        string actual = and.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ToSql()
    {
        And and = new(
        [
            new Parameter(1, "P1"),
            new Parameter(2, "P2"),
            new Column<ColumnTable>("Col1"),
            new Column<ColumnTable>("Col2"),
            new Or (
            [
                new Column<ColumnTable>("ColA"),
                new Column<ColumnTable>("ColB"),
                new Parameter(2, "PA")
            ])
        ]);

        string expected = $"(@P1_1 AND @P2_2 AND [ColumnTable].[Col1] AND [ColumnTable].[Col2] AND ([ColumnTable].[ColA] OR [ColumnTable].[ColB] OR @PA_3))";
        string actual = and.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);

    }

    [Fact]
    public void And_ParameterCount()
    {
        And and = new(
        [
            new Parameter(1, "P1"),
            new Parameter(2, "P2"),
            new Column<ColumnTable>("Col1"),
            new Column<ColumnTable>("Col2"),
            new Or (
            [
                new Column<ColumnTable>("ColA"),
                new Column<ColumnTable>("ColB"),
                new Parameter(2, "PA")
            ])
        ]);

        int actual = and.DescendantParameters.Count();
        int expected = 3;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ColumnsCount()
    {
        And and = new(
        [
            new Parameter(1, "P1"),
            new Parameter(2, "P2"),
            new Column<ColumnTable>("Col1"),
            new Column<ColumnTable>("Col2"),
            new Or (
            [
                new Column<ColumnTable>("ColA"),
                new Column<ColumnTable>("ColB"),
                new Parameter(2, "PA")
            ])
        ]);

        int actual = and.DescendantColumns.Count();
        int expected = 4;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ParameterValues()
    {
        And and = new(
        [
            new Parameter(1, "P1"),
            new Parameter(2, "P2"),
            new Column<ColumnTable>("Col1"),
            new Column<ColumnTable>("Col2"),
            new Or (
            [
                new Column<ColumnTable>("ColA"),
                new Column<ColumnTable>("ColB"),
                new Parameter(3, "PA")
            ])
        ]);


        Parameter p = and.DescendantParameters.Where(p => p.Name == "P1").Single();
        Assert.NotNull(p.Value);
        int actual = (int)p.Value;
        int expected = 1;

        Assert.Equal(expected, actual);

        p = and.DescendantParameters.Where(p => p.Name == "P2").Single();
        Assert.NotNull(p.Value);
        actual = (int)p.Value;
        expected = 2;

        Assert.Equal(expected, actual);

        p = and.DescendantParameters.Where(p => p.Name == "PA").Single();
        Assert.NotNull(p.Value);
        actual = (int)p.Value;
        expected = 3;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ColumnName()
    {
        And and = new(
        [
            new Parameter(1, "P1"),
            new Parameter(2, "P2"),
            new Column<ColumnTable>("Col1"),
            new Column<ColumnTable>("Col2"),
            new Or (
            [
                new Column<ColumnTable>("ColA"),
                new Column<ColumnTable>("ColB"),
                new Parameter(3, "PA")
            ])
        ]);

        //if the column doesn't exist an exception will be throw and the test will fail
        ColumnBase col = and.DescendantColumns.Where(c => c.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[ColA]").Single();
        col = and.DescendantColumns.Where(c => c.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[ColB]").Single();
        col = and.DescendantColumns.Where(c => c.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Col1]").Single();
        col = and.DescendantColumns.Where(c => c.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Col2]").Single();
        col = and.DescendantColumns.Where(c => c.ColumnInfo.ColumnTag == "ColumnTable.Col2").Single();
        col = and.DescendantColumns.Where(c => c.ColumnInfo == "ColumnTable.Col2").Single();
    }

    [Fact]
    public void And_ContainsNullPredicate_ThrowsNullReferenceException() =>
    Assert.Throws<NullReferenceException>(() => new And(
    [
        new Parameter(1, "P1"),
        null!,
    ]));
}