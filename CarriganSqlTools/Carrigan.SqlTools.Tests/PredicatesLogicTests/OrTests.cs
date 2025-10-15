using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

public class OrTests
{
    [Fact]
    public void Or_Empty_ToSql() => 
        Assert.Throws<ArgumentNullException>(() => new Or([]));

    [Fact]
    public void Or_null_ToSql() => 
        Assert.Throws<ArgumentNullException>(() => new Or(null!));

    [Fact]
    public void Or_Single_ToSql()
    {
        And and = new(
        [
                new Parameter("P1", 1),
        ]);

        string expected = $"@Parameter_P1";
        string actual = and.ToSql();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Or_ToSql()
    {
        Or or = new(
        [
            new Parameter("P1", 1),
            new Parameter("P2", 2),
            new Column<ColumnTable>("Col1"),
            new Column<ColumnTable>("Col2"),
            new And (
            [
                new Column<ColumnTable>("ColA"),
                new Column<ColumnTable>("ColB"),
                new Parameter("PA", 2)
            ])
        ]);

        string expected = $"(@Parameter_P1 OR @Parameter_P2 OR [ColumnTable].[Col1] OR [ColumnTable].[Col2] OR ([ColumnTable].[ColA] AND [ColumnTable].[ColB] AND @Parameter_PA))";
        string actual = or.ToSql();

        Assert.Equal(expected, actual);

    }

    [Fact]
    public void Or_ParameterCount()
    {
        Or or = new(
        [
            new Parameter("P1", 1),
            new Parameter("P2", 2),
            new Column<ColumnTable>("Col1"),
            new Column<ColumnTable>("Col2"),
            new And (
            [
                new Column<ColumnTable>("ColA"),
                new Column<ColumnTable>("ColB"),
                new Parameter("PA", 2)
            ])
        ]);

        int actual = or.Parameters.Count();
        int expected = 3;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Or_ParameterValues()
    {
        Or or = new(
        [
            new Parameter("P1", 1),
            new Parameter("P2", 2),
            new Column<ColumnTable>("Col1"),
            new Column<ColumnTable>("Col2"),
            new And (
            [
                new Column<ColumnTable>("ColA"),
                new Column<ColumnTable>("ColB"),
                new Parameter("PA", 3)
            ])
        ]);


        Parameter p = or.Parameters.Where(p => p.Name == "P1").Single(); 
        object? nullableActual = p.Value;
        Assert.NotNull(nullableActual);
        int actual = (int)nullableActual;
        int expected = 1;

        Assert.Equal(expected, actual);

        p = or.Parameters.Where(p => p.Name == "P2").Single();
        nullableActual = p.Value;
        Assert.NotNull(nullableActual);
        actual = (int)nullableActual;
        expected = 2;

        Assert.Equal(expected, actual);

        p = or.Parameters.Where(p => p.Name == "PA").Single();
        nullableActual = p.Value;
        Assert.NotNull(nullableActual);
        actual = (int)nullableActual;
        expected = 3;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Or_ColumnsCount()
    {
        Or or = new(
        [
                new Parameter("P1", 1),
                new Parameter("P2", 2),
                new Column<ColumnTable>("Col1"),
                new Column<ColumnTable>("Col2"),
                new Or (
                [
                    new Column<ColumnTable>("ColA"),
                    new Column<ColumnTable>("ColB"),
                    new Parameter("PA", 2)
                ])
        ]);

        int actual = or.Columns.Count();
        int expected = 4;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Or_ColumnName()
    {
        Or or = new(
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
        ColumnBase col = or.Columns.Where(c => c.ColumnInfo == "[ColumnTable].[ColA]").Single();
        col = or.Columns.Where(c => c.ColumnInfo == "[ColumnTable].[ColB]").Single();
        col = or.Columns.Where(c => c.ColumnInfo == "[ColumnTable].[Col1]").Single();
        col = or.Columns.Where(c => c.ColumnInfo == "[ColumnTable].[Col2]").Single();
    }
}
