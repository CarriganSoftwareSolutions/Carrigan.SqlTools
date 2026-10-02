using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.PredicateLogicTests;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.PredicatesLogicTests;

public  class ColumnsTests : PredicateLogicBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
        [];

    private static readonly PostgreSqlDialect Dialect = new();


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Column<ColumnTable>(nameof(ColumnTable.Col1)),
        new Column<ColumnTable>(nameof(ColumnTable.Col2)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Column<ColumnTable>(nameof(ColumnTable.Col1)),
        new Column<ColumnTable>(nameof(ColumnTable.Col1)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Column<ColumnTable>(nameof(ColumnTable.Col1)),
        new Column<ColumnTable>(nameof(ColumnTable.Col2)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Column<ColumnTable>(nameof(ColumnTable.Col1)),
        new Column<ColumnTable>(nameof(ColumnTable.Col2)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Column<ColumnTable>(nameof(ColumnTable.Col1)),
        new Column<ColumnTable>(nameof(ColumnTable.Col2)),
    ];

    [Fact]
    public void ColumnValues_One_Constructor_NullColumnException_Null_PropertyName() =>
        Assert.Throws<ArgumentNullException>(() => new Column<ColumnTable>((PropertyName)null!));
    [Fact]
    public void ColumnValues_One_Constructor_NullColumnException_Null_String() =>
        Assert.Throws<InvalidPropertyException<ColumnTable>>(() => new Column<ColumnTable>((string)null!));

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
        int actual = cv.AllParticipatingParameters.Count();

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
        int actual = cv.AllParticipatingColumns.OfType<IColumnBase>().Count();

        Assert.Equal(expectedValue, actual);
    }

    [InlineData("Col1", "\"ColumnTable\".\"Col1\"")]
    [InlineData("Col2", "\"ColumnTable\".\"Col2\"")]
    [InlineData("ColA", "\"ColumnTable\".\"ColA\"")]
    [InlineData("ColB", "\"ColumnTable\".\"ColB\"")]
    [Theory]
    public void ColumnValues_One_Constructor_Value_ColumnName(string propertyName, string expectedColumnName)
    {
        Column<ColumnTable> cv = new(propertyName);

        string actual = cv?.ToSqlFragments(Dialect)?.ToSql(Dialect) ?? string.Empty ;

        Assert.Equal(expectedColumnName, actual);
    }

    [Fact]
    public void ColumnValues_Get()
    {
        string[] propertyNames = ["Col1", "Col2", "ColA", "ColB"];
        IEnumerable<Column<ColumnTable>> columnValues = propertyNames.Select(propertyName => new Column<ColumnTable>(propertyName));

        foreach (string columnName in propertyNames)
        {
            _ = columnValues.Single(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == $"\"ColumnTable\".\"{columnName}\"");
            _ = columnValues.Single(col => col.ColumnInfo.ToString() == $"ColumnTable.{columnName}");
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

        Assert.Equal($"\"ColumnTable\".\"{ expectedColumnName}\"", column.ColumnInfo.ColumnTag.ToSql(Dialect));
        Assert.Equal($"ColumnTable.{expectedColumnName}", column.ColumnInfo.ToString());
        Assert.Equal($"ColumnTable.{expectedColumnName}", column.ColumnInfo.ColumnTag.ToString());
    }

    [Fact]
    public void ColumnValues_PropertyNameConstructor_Null_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new Column<ColumnTable>((PropertyName)null!));

}
