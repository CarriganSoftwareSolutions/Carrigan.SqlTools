using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

//IGNORE SPELLING: ilsabasbdyas

public class ColumnValueTests
{
    [Fact]
    public void ByColumnValue_ConstructorSimple_InValid_BadCol() => 
        Assert.Throws<ArgumentException>(() => new ColumnValues<ColumnTable>("ilsabasbdyas", "1"));

    [Fact]
    public void ByColumnValue_ConstructorSimple_Valid() => 
        _ = new ColumnValues<ColumnTable>(nameof(ColumnTable.Col1), "1");

    [Fact]
    public void ByColumnValue_ConstructorSimple_ParameterCount()
    {
        ColumnValues<ColumnTable> byColoumnValues = new(nameof(ColumnTable.Col1), "1");
        int expected = 1;
        int actual = byColoumnValues.Parameter.Count();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_ParameterValidate()
    {
        ColumnValues<ColumnTable> byColoumnValues = new(nameof(ColumnTable.Col1), "1");

        string expectedValue;
        object actualValue;
        string expectedString;
        string actualString;

        Parameters parameter;

        parameter = byColoumnValues.Parameter.Where(param => param.Name == "Col1").First();
        expectedValue = "1";
        expectedString = "Col1";
        actualValue = parameter.Value ?? string.Empty;
        actualString = parameter.Name;
        Assert.Equal(expectedValue, actualValue);
        Assert.Equal(expectedString, actualString);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_ColumnCount()
    {
        ColumnValues<ColumnTable> byColoumnValues = new(nameof(ColumnTable.Col1), "1");
        int expected = 1;
        int actual = byColoumnValues.Column.Count();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_Validate()
    {
        ColumnValues<ColumnTable> byColoumnValues = new(nameof(ColumnTable.Col1), "1");
        string expectedString;
        string actualString;

        IColumnValue column;

        column = byColoumnValues.Column.Where(col => col.ColumnTag.ToString() == "[ColumnTable].[Col1]").First();
        expectedString = "[ColumnTable].[Col1]";
        actualString = column.ColumnTag.ToString();
        Assert.Equal(expectedString, actualString);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_ToSql()
    {
        ColumnValues<ColumnTable> byColoumnValues = new(nameof(ColumnTable.Col1), "1");
        string expectedString = "([ColumnTable].[Col1] = @Parameter_Col1)";
        string actualString = byColoumnValues.ToSql();
        Assert.Equal(expectedString, actualString);
    }
}
