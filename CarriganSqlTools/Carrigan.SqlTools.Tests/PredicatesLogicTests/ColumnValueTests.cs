using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
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
        int actual = byColumnValues.DescendantParameters.Count();

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

        parameter = byColumnValues.DescendantParameters.Where(param => param.Name == "Col1").First();
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
        int actual = byColumnValues.DescendantColumns.Count();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_Validate()
    {
        ColumnValue<ColumnTable> byColumnValues = new(nameof(ColumnTable.Col1), "1");
        string expectedString;
        string actualString;

        ColumnBase column;

        column = byColumnValues.DescendantColumns.Where(col => col.ColumnInfo.ToString() == "[ColumnTable].[Col1]").First();
        expectedString = "[ColumnTable].[Col1]";
        actualString = column.ColumnInfo.ToString();
        Assert.Equal(expectedString, actualString);
    }

    [Fact]
    public void ByColumnValue_ConstructorSimple_ToSql()
    {
        ColumnValue<ColumnTable> byColumnValues = new(nameof(ColumnTable.Col1), "1");
        string expectedString = "([ColumnTable].[Col1] = @Col1_1)";
        string actualString = byColumnValues.ToSqlFragments().ToSql(new SqlServerDialect());
        Assert.Equal(expectedString, actualString);
    }

    [Fact]
    public void ByColumnValue_PropertyNameNull_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new ColumnValue<ColumnTable>((PropertyName)null!, "1"));

}