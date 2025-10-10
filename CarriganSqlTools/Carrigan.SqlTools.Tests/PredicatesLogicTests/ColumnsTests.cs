using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

public  class ColumnsTests
{
    [Fact]
    public void ColumnValues_One_Constructor_NullColumnException_Null() =>
        Assert.Throws<ArgumentNullException>(() => new Column<ColumnTable>(null!));

    [Fact]
    public void ColumnValues_One_Constructor_NullColumnException_EmptyString() =>
        Assert.Throws<InvalidPropertyException<ColumnTable>>(() => new Column<ColumnTable>(string.Empty));

    [Fact]
    public void ColumnValues_One_Constructor_Column_DoesNot_Exist() =>
        Assert.Throws<InvalidPropertyException<ColumnTable>>(() => new Column<ColumnTable>("C#"));

    [Theory]
    [InlineData("Col1")]
    [InlineData("Col2")]
    [InlineData("ColA")]
    [InlineData("ColB")]
    public void ColumnValues_One_Constructor_Value_ParameterCount(string propertyName)
    {
        Column<ColumnTable> cv = new(propertyName);
        int expectedValue = 0;
        int actual = cv.Parameters.Count();

        Assert.Equal(expectedValue, actual);
    }

    [Theory]
    [InlineData("Col1")]
    [InlineData("Col2")]
    [InlineData("ColA")]
    [InlineData("ColB")]
    public void ColumnValues_One_Constructor_Value_ColumnCount(string propertyName)
    {
        Column<ColumnTable> cv = new(propertyName);
        int expectedValue = 1;
        int actual = cv.Columns.Count();

        Assert.Equal(expectedValue, actual);
    }

    [InlineData("Col1", "Col1")]
    [InlineData("Col2", "Col2")]
    [InlineData("ColA", "ColA")]
    [InlineData("ColB", "ColB")]
    [Theory]
    public void ColumnValues_One_Constructor_Value_ColumnName(string propertyName, string expectedColumnName)
    {
        Column<ColumnTable> cv = new(propertyName);


        _ = cv.Columns.Where(col => col.ColumnInfo == $"[ColumnTable].[{expectedColumnName}]").Single();
    }

    [Fact]
    public void ColumnValues_Get()
    {
        string[] propertyNames = ["Col1", "Col2", "ColA", "ColB"];
        IEnumerable<Column<ColumnTable>> columnValues = propertyNames.Select(propertyName => new Column<ColumnTable>(propertyName));

        foreach (string columnName in propertyNames)
        {
            _ = columnValues.Single(col => col.ColumnInfo == $"[ColumnTable].[{columnName}]");
        }
    }
    [Theory]
    [InlineData("Col1", "Col1")]
    [InlineData("Col2", "Col2")]
    [InlineData("ColA", "ColA")]
    [InlineData("ColB", "ColB")]
    [InlineData("Pizza", "Pizza")]
    [InlineData("D000destruct0", "D000destruct0")]
    [InlineData("Express", "Express")]
    public void Columns_Tests(string propertyName, string expectedColumnName)
    {
        Column<ColumnTable> column = new(propertyName);

        Assert.Equal($"[ColumnTable].[{ expectedColumnName}]", column.ColumnInfo);
    }
}
