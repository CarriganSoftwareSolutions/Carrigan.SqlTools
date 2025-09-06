using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

public class AndTests
{
    [Fact]
    public void And_Empty_ToSql()
    {
        Assert.Throws<ArgumentNullException>(() => new And([]));
    }

    [Fact]
    public void And_null_ToSql()
    {
        Assert.Throws<ArgumentNullException>(() => new And(null!));
    }

    [Fact]
    public void And_Single_ToSql()
    {
        And and = new(
        [
            new Parameters("P1", 1),
        ]);

        string expected = $"@Parameter_P1";
        string actual = and.ToSql();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ToSql()
    {
        And and = new(
        [
            new Parameters("P1", 1),
            new Parameters("P2", 2),
            new Columns<ColumnTable>("Col1"),
            new Columns<ColumnTable>("Col2"),
            new Or (
            [
                new Columns < ColumnTable >("ColA"),
                new Columns<ColumnTable>("ColB"),
                new Parameters("PA", 2)
            ])
        ]);

        string expected = $"(@Parameter_P1 AND @Parameter_P2 AND [ColumnTable].[Col1] AND [ColumnTable].[Col2] AND ([ColumnTable].[ColA] OR [ColumnTable].[ColB] OR @Parameter_PA))";
        string actual = and.ToSql();

        Assert.Equal(expected, actual);

    }

    [Fact]
    public void And_ParameterCount()
    {
        And and = new(
        [
            new Parameters("P1", 1),
            new Parameters("P2", 2),
            new Columns < ColumnTable >("Col1"),
            new Columns < ColumnTable >("Col2"),
            new Or (
            [
                new Columns < ColumnTable >("ColA"),
                new Columns <ColumnTable>("ColB"),
                new Parameters("PA", 2)
            ])
        ]);

        int actual = and.Parameter.Count();
        int expected = 3;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ColumnsCount()
    {
        And and = new(
        [
            new Parameters("P1", 1),
            new Parameters("P2", 2),
            new Columns<ColumnTable>("Col1"),
            new Columns<ColumnTable>("Col2"),
            new Or (
            [
                new Columns<ColumnTable>("ColA"),
                new Columns<ColumnTable>("ColB"),
                new Parameters("PA", 2)
            ])
        ]);

        int actual = and.Column.Count();
        int expected = 4;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ParameterValues()
    {
        And and = new(
        [
            new Parameters("P1", 1),
            new Parameters("P2", 2),
            new Columns<ColumnTable>("Col1"),
            new Columns<ColumnTable>("Col2"),
            new Or (
            [
                new Columns<ColumnTable>("ColA"),
                new Columns<ColumnTable>("ColB"),
                new Parameters("PA", 3)
            ])
        ]);


        Parameters p = and.Parameter.Where(p => p.Name == "P1").Single();
        Assert.NotNull(p.Value);
        int actual = (int)p.Value;
        int expected = 1;

        Assert.Equal(expected, actual);

        p = and.Parameter.Where(p => p.Name == "P2").Single();
        Assert.NotNull(p.Value);
        actual = (int)p.Value;
        expected = 2;

        Assert.Equal(expected, actual);

        p = and.Parameter.Where(p => p.Name == "PA").Single();
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
            new Parameters("P1", 1),
            new Parameters("P2", 2),
            new Columns<ColumnTable>("Col1"),
            new Columns<ColumnTable>("Col2"),
            new Or (
            [
                new Columns<ColumnTable>("ColA"),
                new Columns<ColumnTable>("ColB"),
                new Parameters("PA", 3)
            ])
        ]);

        //if the column doesn't exist an exception will be throw and the test will fail
        IColumnValue col = and.Column.Where(c => c.ColumnTag == "[ColumnTable].[ColA]").Single();
        col = and.Column.Where(c => c.ColumnTag == "[ColumnTable].[ColB]").Single();
        col = and.Column.Where(c => c.ColumnTag == "[ColumnTable].[Col1]").Single();
        col = and.Column.Where(c => c.ColumnTag == "[ColumnTable].[Col2]").Single();
    }
}
