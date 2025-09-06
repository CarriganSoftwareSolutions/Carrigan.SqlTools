using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

public  class ColumnValuesTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ColumnValues_One_Constructor_NullColumnException(string? column)
    {
        Assert.Throws<ArgumentException>(() => new Columns<ColumnTable>(column!));
    }

    [Fact]
    public void ColumnValues_One_Constructor_Column_DoesNot_Exist()
    {
        Assert.Throws<ArgumentException>(() => new Columns<ColumnTable>("C#"));
    }

    [Theory]
    [InlineData("Col1")]
    [InlineData("Col2")]
    [InlineData("ColA")]
    [InlineData("ColB")]
    public void ColumnValues_One_Constructor_Value_ParameterCount(string columnName)
    {
        Columns<ColumnTable> cv = new(columnName);
        int expectvalue = 0;
        int actual = cv.Parameter.Count();

        Assert.Equal(expectvalue, actual);
    }

    [Theory]
    [InlineData("Col1")]
    [InlineData("Col2")]
    [InlineData("ColA")]
    [InlineData("ColB")]
    public void ColumnValues_One_Constructor_Value_ColumnCount(string columnName)
    {
        Columns<ColumnTable> cv = new(columnName);
        int expectvalue = 1;
        int actual = cv.Column.Count();

        Assert.Equal(expectvalue, actual);
    }

    [InlineData("Col1")]
    [InlineData("Col2")]
    [InlineData("ColA")]
    [InlineData("ColB")]
    [Theory]
    public void ColumnValues_One_Constructor_Value_ColumnName(string columnName)
    {
        Columns<ColumnTable> cv = new(columnName);


        _ = cv.Column.Where(col => col.ColumnTag == $"[ColumnTable].[{columnName}]").Single();
    }

    [Fact]
    public void ColumnValues_Get()
    {
        string[] columns = ["Col1", "Col2", "ColA", "ColB"];
        IEnumerable<Columns<ColumnTable>> columnValues = columns.Select(column => new Columns<ColumnTable>(column));

        foreach (string columnName in columns)
        {
            _ = columnValues.Where(col => col.ColumnTag == $"[ColumnTable].[{columnName}]").Single();
        }
    }
    [Theory]
    [InlineData("Col1")]
    [InlineData("Col2")]
    [InlineData("ColA")]
    [InlineData("ColB")]
    [InlineData("Pizza")]
    [InlineData("D000descruct0")]
    [InlineData("Express")]
    public void Columns_Tests(string columnName)
    {
        Columns<ColumnTable> column = new(columnName);

        Assert.Equal(columnName, column.Name);
    }

    [Theory]
    [InlineData("Col1")]
    [InlineData("Col2")]
    [InlineData("ColA")]
    [InlineData("ColB")]
    [InlineData("Pizza")]
    [InlineData("D000descruct0")]
    [InlineData("Express")]
    public void Columns_Tests_Bulk_No_Arguments(string columnName)
    {
        IEnumerable<Columns<ColumnTable>> columns = Columns<ColumnTable>.Get();

        Assert.True(columns.Where(column => column.Name == columnName).Any());
    }

    [Theory]
    [InlineData("Pizza")]
    [InlineData("D000descruct0")]
    [InlineData("Express")]
    public void Columns_Tests_Bulk_Limited(string columnName)
    {
        IEnumerable<Columns<ColumnTable>> columns = Columns<ColumnTable>.Get("Pizza", "Express", "D000descruct0");

        Assert.True(columns.Where(column => column.Name == columnName).Any());
    }

    [Theory]
    [InlineData("Col1")]
    [InlineData("Col2")]
    [InlineData("ColA")]
    [InlineData("ColB")]
    public void Columns_Tests_Bulk_Limited_False(string columnName)
    {
        IEnumerable<Columns<ColumnTable>> columns = Columns<ColumnTable>.Get("Pizza", "Express", "D000descruct0");

        Assert.False(columns.Where(column => column.Name == columnName).Any());
    }
}
