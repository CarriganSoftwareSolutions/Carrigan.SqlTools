using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

//IGNORE SPELLING: ilsabasbdyas

public class ColumnValueTests
{
    [Fact]
    public void ByColumnValue_ConstructorSimple_InValid_BadCol() => 
        Assert.Throws<InvalidPropertyException<ColumnTable>>(() => new ColumnValue<ColumnTable>("ilsabasbdyas", "1"));

    [Fact]
    public void ByColumnValue_ConstructorSimple_Valid() => 
        _ = new ColumnValue<ColumnTable>(nameof(ColumnTable.Col1), "1");

    [Fact]
    public void ByColumnValue_ConstructorSimple_ParameterCount()
    {
        ColumnValue<ColumnTable> byColumnValues = new(nameof(ColumnTable.Col1), "1");
        int expected = 1;
        int actual = byColumnValues.Parameters.Count();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_ParameterValidate()
    {
        ColumnValue<ColumnTable> byColumnValues = new(nameof(ColumnTable.Col1), "1");

        string expectedValue;
        object actualValue;
        string expectedString;
        string actualString;

        Parameter parameter;

        parameter = byColumnValues.Parameters.Where(param => param.Name == "Col1").First();
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
        ColumnValue<ColumnTable> byColumnValues = new(nameof(ColumnTable.Col1), "1");
        int expected = 1;
        int actual = byColumnValues.Columns.Count();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_Validate()
    {
        ColumnValue<ColumnTable> byColumnValues = new(nameof(ColumnTable.Col1), "1");
        string expectedString;
        string actualString;

        IColumn column;

        column = byColumnValues.Columns.Where(col => col.ColumnInfo.ToString() == "[ColumnTable].[Col1]").First();
        expectedString = "[ColumnTable].[Col1]";
        actualString = column.ColumnInfo.ToString();
        Assert.Equal(expectedString, actualString);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_ToSql()
    {
        ColumnValue<ColumnTable> byColumnValues = new(nameof(ColumnTable.Col1), "1");
        string expectedString = "([ColumnTable].[Col1] = @Parameter_Col1)";
        string actualString = byColumnValues.ToSql();
        Assert.Equal(expectedString, actualString);
    }
}
