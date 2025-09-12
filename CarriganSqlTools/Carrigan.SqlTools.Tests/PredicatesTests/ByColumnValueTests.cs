using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

//IGNORE SPELLING: ilsabasbdyas

public class ByColumnValueTests
{
    #region constructor invalid empty
    [Fact]
    public void ByColumnValue_ConstructorComposite_InValid_Empty()
    {
        Dictionary<string, object> dictionary = new([]);
        _ = Assert.Throws<ArgumentException>(() => new ColumnValues<ColumnTable>(dictionary));
    }
    [Fact]
    public void ByColumnValue_ConstructorKeyValuePair_InValid_Empty()
    {
        _ = Assert.Throws<ArgumentException>(() => new ColumnValues<ColumnTable>(Array.Empty<KeyValuePair<string, object>>()));
    }
    [Fact]
    public void ByColumnValue_ByMultipleValues_InValid_Empty()
    {
        _ = Assert.Throws<ArgumentException>(() => ColumnValues<ColumnTable>.ByMultipleValues("ilsabasbdyas"));
    }
    #endregion

    #region constructor invalid bad column
    [Fact]
    public void ByColumnValue_ConstructorSimple_InValid_BadCol()
    {
        _ = Assert.Throws<SqlIdentifierException>(() => new ColumnValues<ColumnTable>("ilsabasbdyas", "1"));
    }
    [Fact]
    public void ByColumnValue_ConstructorComposite_InValid_BadCol()
    {
        Dictionary<string, object> dictionary = new([new("ilsabasbdyas", "1"), new(nameof(ColumnTable.Col2), "2")]);
        _ = Assert.Throws<SqlIdentifierException>(() => new ColumnValues<ColumnTable>(dictionary));
    }
    [Fact]
    public void ByColumnValue_ConstructorKeyValuePair_InValid_BadCol()
    {
        _ = Assert.Throws<SqlIdentifierException>(() => new ColumnValues<ColumnTable>([new("ilsabasbdyas", "1"), new(nameof(ColumnTable.Col2), "2")]));
    }
    [Fact]
    public void ByColumnValue_ByMultipleValues_InValid_BadCol()
    {
        _ = Assert.Throws<SqlIdentifierException>(() => ColumnValues<ColumnTable>.ByMultipleValues("ilsabasbdyas", "1", "2"));
    }
    #endregion

    #region constructor valid
    [Fact]
    public void ByColumnValue_ConstructorSimple_Valid()
    {
        _ = new ColumnValues<ColumnTable>(nameof(ColumnTable.Col1), "1");
    }
    [Fact]
    public void ByColumnValue_ConstructorComposite_Valid()
    {
        Dictionary<string, object> dictionary = new([new(nameof(ColumnTable.Col1), "1"), new(nameof(ColumnTable.Col2), "2")]);
        _ = new ColumnValues<ColumnTable>(dictionary);
    }
    [Fact]
    public void ByColumnValue_ConstructorKeyValuePair_Valid()
    {
        _ = new ColumnValues<ColumnTable>([new(nameof(ColumnTable.Col1), "1"), new(nameof(ColumnTable.Col2), "2")]);
    }
    [Fact]
    public void ByColumnValue_ByMultipleValues_Valid()
    {
        _ = ColumnValues<ColumnTable>.ByMultipleValues(nameof(ColumnTable.Col1), "1", "2");
    }
    #endregion

    #region parameter count

    [Fact]
    public void ByColumnValue_ConstructorSimple_ParameterCount()
    {
        ColumnValues<ColumnTable> byColoumnValues = new(nameof(ColumnTable.Col1), "1");
        int expected = 1;
        int actual = byColoumnValues.Parameter.Count();

        Assert.Equal(expected, actual);
    }
    [Fact]
    public void ByColumnValue_ConstructorComposite_ParameterCount()
    {
        Dictionary<string, object> dictionary = new([new(nameof(ColumnTable.Col1), "1"), new(nameof(ColumnTable.Col2), "2")]);
        ColumnValues<ColumnTable> byColoumnValues = new(dictionary);

        int expected = 2;
        int actual = byColoumnValues.Parameter.Count();

        Assert.Equal(expected, actual);
    }
    [Fact]
    public void ByColumnValue_ConstructorKeyValuePair_ParameterCount()
    {
        ColumnValues<ColumnTable> byColoumnValues =
            new([new(nameof(ColumnTable.Col1), "1"), new(nameof(ColumnTable.Col2), "2")]);
        int expected = 2;
        int actual = byColoumnValues.Parameter.Count();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ByColumnValue_ByMultiValue_ParameterCount()
    {
        PredicatesBase predicate = ColumnValues<ColumnTable>.ByMultipleValues(nameof(ColumnTable.Col1), "1", "2");
        int expected = 2;
        int actual = predicate.Parameter.Count();

        Assert.Equal(expected, actual);
    }
    #endregion

    #region parameter validation

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
        actualValue = parameter.Value;
        actualString = parameter.Name;
        Assert.Equal(expectedValue, actualValue);
        Assert.Equal(expectedString, actualString);
    }
    [Fact]
    public void ByColumnValue_ConstructorComposite_ParameterValidate()
    {
        Dictionary<string, object> dictionary = new([new(nameof(ColumnTable.Col1), "1"), new(nameof(ColumnTable.Col2), "2")]);
        ColumnValues<ColumnTable> byColoumnValues = new(dictionary);

        string expectedValue;
        object actualValue;
        string expectedString;
        string actualString;

        Parameters parameter;

        parameter = byColoumnValues.Parameter.Where(param => param.Name == "Col1").First();
        expectedValue = "1";
        expectedString = "Col1";
        actualValue = parameter.Value;
        actualString = parameter.Name;
        Assert.Equal(expectedValue, actualValue);
        Assert.Equal(expectedString, actualString);

        parameter = byColoumnValues.Parameter.Where(param => param.Name == "Col2").First();
        expectedValue = "2";
        expectedString = "Col2";
        actualValue = parameter.Value;
        actualString = parameter.Name;
        Assert.Equal(expectedValue, actualValue);
        Assert.Equal(expectedString, actualString);
    }
    [Fact]
    public void ByColumnValue_ConstructorKeyValuePair_ParameterValidate()
    {
        ColumnValues<ColumnTable> byColoumnValues =
            new([new(nameof(ColumnTable.Col1), "1"), new(nameof(ColumnTable.Col2), "2")]);

        string expectedValue;
        object actualValue;
        string expectedString;
        string actualString;

        Parameters parameter;

        parameter = byColoumnValues.Parameter.Where(param => param.Name == "Col1").First();
        expectedValue = "1";
        expectedString = "Col1";
        actualValue = parameter.Value;
        actualString = parameter.Name;
        Assert.Equal(expectedValue, actualValue);
        Assert.Equal(expectedString, actualString);

        parameter = byColoumnValues.Parameter.Where(param => param.Name == "Col2").First();
        expectedValue = "2";
        expectedString = "Col2";
        actualValue = parameter.Value;
        actualString = parameter.Name;
        Assert.Equal(expectedValue, actualValue);
        Assert.Equal(expectedString, actualString);

    }

    [Fact]
    public void ByColumnValue_ByMultipleValue_ParameterValidate()
    {
        PredicatesBase predicate = ColumnValues<ColumnTable>.ByMultipleValues(nameof(ColumnTable.Col1), "1", "2");

        string expectedValue;
        object actualValue;
        string expectedString;
        string actualString;

        Parameters parameter;

        parameter = predicate.Parameter.Where(param => param.Name == "Col1").First();
        expectedValue = "1";
        expectedString = "Col1";
        actualValue = parameter.Value;
        actualString = parameter.Name;
        Assert.Equal(expectedValue, actualValue);
        Assert.Equal(expectedString, actualString);

        parameter = predicate.Parameter.Where(param => param.Name == "Col1").Last();
        expectedValue = "2";
        expectedString = "Col1";
        actualValue = parameter.Value;
        actualString = parameter.Name;
        Assert.Equal(expectedValue, actualValue);
        Assert.Equal(expectedString, actualString);

    }
    #endregion

    #region column count

    [Fact]
    public void ByColumnValue_ConstructorSimple_ColumnCount()
    {
        ColumnValues<ColumnTable> byColoumnValues = new(nameof(ColumnTable.Col1), "1");
        int expected = 1;
        int actual = byColoumnValues.Column.Count();

        Assert.Equal(expected, actual);
    }
    [Fact]
    public void ByColumnValue_ConstructorComposite_ColumnCount()
    {
        Dictionary<string, object> dictionary = new([new(nameof(ColumnTable.Col1), "1"), new(nameof(ColumnTable.Col2), "2")]);
        ColumnValues<ColumnTable> byColoumnValues = new(dictionary);

        int expected = 2;
        int actual = byColoumnValues.Column.Count();

        Assert.Equal(expected, actual);
    }
    [Fact]
    public void ByColumnValue_ConstructorKeyValuePair_ColumnCount()
    {
        ColumnValues<ColumnTable> byColoumnValues =
            new([new(nameof(ColumnTable.Col1), "1"), new(nameof(ColumnTable.Col2), "2")]);
        int expected = 2;
        int actual = byColoumnValues.Column.Count();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ByColumnValue_ByMultipleValue_ColumnCount()
    {
        PredicatesBase predicate = ColumnValues<ColumnTable>.ByMultipleValues(nameof(ColumnTable.Col1), "1", "2");
        int expected = 2;
        int actual = predicate.Column.Count();

        Assert.Equal(expected, actual);
    }
    #endregion

    #region column validation

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
    public void ByColumnValue_ConstructorComposite_Validate()
    {
        Dictionary<string, object> dictionary = new([new(nameof(ColumnTable.Col1), "1"), new(nameof(ColumnTable.Col2), "2")]);
        ColumnValues<ColumnTable> byColoumnValues = new(dictionary);
        string expectedString;
        string actualString;

        IColumnValue column;

        column = byColoumnValues.Column.Where(col => col.ColumnTag.ToString() == "[ColumnTable].[Col1]").First();
        expectedString = "[ColumnTable].[Col1]";
        actualString = column.ColumnTag.ToString();
        Assert.Equal(expectedString, actualString);

        column = byColoumnValues.Column.Where(col => col.ColumnTag.ToString() == "[ColumnTable].[Col2]").First();
        expectedString = "[ColumnTable].[Col2]";
        actualString = column.ColumnTag.ToString();
        Assert.Equal(expectedString, actualString);
    }
    [Fact]
    public void ByColumnValue_ConstructorKeyValuePair_ColumnValidate()
    {
        ColumnValues<ColumnTable> byColoumnValues =
            new([new(nameof(ColumnTable.Col1), "1"), new(nameof(ColumnTable.Col2), "2")]);
        string expectedString;
        string actualString;

        IColumnValue column;

        column = byColoumnValues.Column.Where(col => col.ColumnTag.ToString() == "[ColumnTable].[Col1]").First();
        expectedString = "[ColumnTable].[Col1]";
        actualString = column.ColumnTag.ToString();
        Assert.Equal(expectedString, actualString);

        column = byColoumnValues.Column.Where(col => col.ColumnTag.ToString() == "[ColumnTable].[Col2]").First();
        expectedString = "[ColumnTable].[Col2]";
        actualString = column.ColumnTag.ToString();
        Assert.Equal(expectedString, actualString);
    }

    [Fact]
    public void ByColumnValue_ByMultipleValue_ColumnValidate()
    {
        PredicatesBase predicate = ColumnValues<ColumnTable>.ByMultipleValues(nameof(ColumnTable.Col1), "1", "2");
        string expectedString;
        string actualString;

        IColumnValue column;

        column = predicate.Column.Where(col => col.ColumnTag.ToString() == "[ColumnTable].[Col1]").First();
        expectedString = "[ColumnTable].[Col1]";
        actualString = column.ColumnTag.ToString();
        Assert.Equal(expectedString, actualString);

        column = predicate.Column.Where(col => col.ColumnTag.ToString() == "[ColumnTable].[Col1]").Last();
        expectedString = "[ColumnTable].[Col1]";
        actualString = column.ColumnTag.ToString();
        Assert.Equal(expectedString, actualString);
    }
    #endregion

    #region ToSql

    [Fact]
    public void ByColumnValue_ConstructorSimple_ToSql()
    {
        ColumnValues<ColumnTable> byColoumnValues = new(nameof(ColumnTable.Col1), "1");
        string expectedString = "([ColumnTable].[Col1] = @Parameter_Col1)";
        string actualString = byColoumnValues.ToSql();
        Assert.Equal(expectedString, actualString);
    }
    [Fact]
    public void ByColumnValue_ConstructorComposite_ToSql()
    {
        Dictionary<string, object> dictionary = new([new(nameof(ColumnTable.Col1), "1"), new(nameof(ColumnTable.Col2), "2")]);
        ColumnValues<ColumnTable> byColoumnValues = new(dictionary);
        string expectedString = "(([ColumnTable].[Col1] = @Parameter_Col1) AND ([ColumnTable].[Col2] = @Parameter_Col2))";
        string actualString = byColoumnValues.ToSql();
        Assert.Equal(expectedString, actualString);
    }
    [Fact]
    public void ByColumnValue_ConstructorKeyValuePair_ToSql()
    {
        ColumnValues<ColumnTable> byColoumnValues =
            new([new(nameof(ColumnTable.Col1), "1"), new(nameof(ColumnTable.Col2), "2")]);
        string expectedString = "(([ColumnTable].[Col1] = @Parameter_Col1) AND ([ColumnTable].[Col2] = @Parameter_Col2))";
        string actualString = byColoumnValues.ToSql();
        Assert.Equal(expectedString, actualString);
    }
    [Fact]
    public void ByColumnValue_ByMultipleValue_ToSql()
    {
        PredicatesBase predicate = ColumnValues<ColumnTable>.ByMultipleValues(nameof(ColumnTable.Col1), "1", "2");
        string expectedString = "(([ColumnTable].[Col1] = @Parameter_0_R_Col1) OR ([ColumnTable].[Col1] = @Parameter_1_R_Col1))";
        string actualString = predicate.ToSql();
        Assert.Equal(expectedString, actualString);
    }
    #endregion
}
