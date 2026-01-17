using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

public class AndTests
{
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
            new Parameter("P1", 1, null),
        ]);

        string expected = $"@Parameter_P1";
        string actual = and.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ToSql()
    {
        And and = new(
        [
            new Parameter("P1", 1, null),
            new Parameter("P2", 2, null),
            new Column<ColumnTable>("Col1"),
            new Column<ColumnTable>("Col2"),
            new Or (
            [
                new Column < ColumnTable >("ColA"),
                new Column<ColumnTable>("ColB"),
                new Parameter("PA", 2, null)
            ])
        ]);

        string expected = $"(@Parameter_P1 AND @Parameter_P2 AND [ColumnTable].[Col1] AND [ColumnTable].[Col2] AND ([ColumnTable].[ColA] OR [ColumnTable].[ColB] OR @Parameter_PA))";
        string actual = and.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expected, actual);

    }

    [Fact]
    public void And_ParameterCount()
    {
        And and = new(
        [
            new Parameter("P1", 1, null),
            new Parameter("P2", 2, null),
            new Column < ColumnTable >("Col1"),
            new Column < ColumnTable >("Col2"),
            new Or (
            [
                new Column < ColumnTable >("ColA"),
                new Column <ColumnTable>("ColB"),
                new Parameter("PA", 2, null)
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
            new Parameter("P1", 1, null),
            new Parameter("P2", 2, null),
            new Column<ColumnTable>("Col1"),
            new Column<ColumnTable>("Col2"),
            new Or (
            [
                new Column<ColumnTable>("ColA"),
                new Column<ColumnTable>("ColB"),
                new Parameter("PA", 2, null)
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
            new Parameter("P1", 1, SqlTypeDefinition.AsInt()),
            new Parameter("P2", 2, SqlTypeDefinition.AsInt()),
            new Column<ColumnTable>("Col1"),
            new Column<ColumnTable>("Col2"),
            new Or (
            [
                new Column<ColumnTable>("ColA"),
                new Column<ColumnTable>("ColB"),
                new Parameter("PA", 3, SqlTypeDefinition.AsInt())
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
            new Parameter("P1", 1),
            new Parameter("P2", 2),
            new Column<ColumnTable>("Col1"),
            new Column<ColumnTable>("Col2"),
            new Or (
            [
                new Column<ColumnTable>("ColA"),
                new Column<ColumnTable>("ColB"),
                new Parameter("PA", 3)
            ])
        ]);

        //if the column doesn't exist an exception will be throw and the test will fail
        ColumnBase col = and.DescendantColumns.Where(c => c.ColumnInfo == "[ColumnTable].[ColA]").Single();
        col = and.DescendantColumns.Where(c => c.ColumnInfo == "[ColumnTable].[ColB]").Single();
        col = and.DescendantColumns.Where(c => c.ColumnInfo == "[ColumnTable].[Col1]").Single();
        col = and.DescendantColumns.Where(c => c.ColumnInfo == "[ColumnTable].[Col2]").Single();
    }
}
