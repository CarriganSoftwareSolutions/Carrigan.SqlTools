using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests;

public abstract class ColumnBaseTests<modelT> where modelT : class
{
    protected abstract ISqlDialects Dialect { get; }

    protected abstract string? SchemaName { get; }

    protected abstract string TableName { get; }

    protected abstract ColumnBase NewColumn(string propertyName);

    protected abstract ColumnBase NewColumn(PropertyName propertyName);

    protected abstract SqlExpression NewColumnAsExpression(string propertyName);

    protected abstract SqlExpression NewColumnAsExpression(PropertyName propertyName);

    protected KeyValuePair<string, ColumnName> NewKvp(string propertyName, string ColumnName) =>
        new (propertyName, new ColumnName(ColumnName));

    protected KeyValuePair<string, ColumnName> NewKvp(string propertyName) =>
        new(propertyName, new ColumnName(propertyName));

    internal Dictionary<string, ColumnTag> ExpectedPropertyColumnTag =>
    new
    (
        ExpectedPropertyColumnName.
            Keys.
            Select(key => new KeyValuePair<string, ColumnTag>(key, new ColumnTag(new TableTag(SchemaName, TableName), ExpectedPropertyColumnName[key])))
    );

    internal abstract Dictionary<string, ColumnName> ExpectedPropertyColumnName { get; }

    protected virtual IEnumerable<string> NotMappedProperties => [];

    protected TableTag ExpectedTableTag =>
        new(SchemaName, TableName);

    protected TableName ExpectedTableName =>
        new(TableName);

    protected SchemaName? ExpectedScehemaName =>
        IdentifierTypes.SchemaName.New(SchemaName);

    protected abstract string ExpectSqlFragment(string expectedColumnName);

    protected ColumnBaseTests()
    {
    }

    protected void ValidateSqlFragment(string propertyName)
    {
        ColumnBase columnBase = NewColumn(propertyName);

        string actual = columnBase?.ToSqlFragments(Dialect)?.ToSql(Dialect) ?? string.Empty;
        string expected = ExpectSqlFragment(ExpectedPropertyColumnName[propertyName]);

        Assert.Equal(expected, actual);

        columnBase = NewColumn(new PropertyName(propertyName));

        actual = columnBase?.ToSqlFragments(Dialect)?.ToSql(Dialect) ?? string.Empty;

        Assert.Equal(expected, actual);

        SqlExpression sqlExpression = NewColumnAsExpression(propertyName);

        actual = sqlExpression.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);

        sqlExpression = NewColumnAsExpression(new PropertyName(propertyName));

        actual = sqlExpression.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    protected void ValidateExpectedPropertyColumnTag(string propertyName)
    {
        ColumnBase columnBase = NewColumn(propertyName);
        ColumnTag actual = columnBase.ColumnInfo.ColumnTag;
        ColumnTag expected = ExpectedPropertyColumnTag[propertyName];
        Assert.Equal(expected, actual);

        columnBase = NewColumn(new PropertyName(propertyName));
        actual = columnBase.ColumnInfo.ColumnTag;
        Assert.Equal(expected, actual);
    }

    protected void ValidateExpectedPropertyColumnName(string propertyName)
    {
        ColumnBase columnBase = NewColumn(propertyName);
        ColumnName actual = columnBase.ColumnInfo.ColumnTag.ColumnName;
        ColumnName expected = ExpectedPropertyColumnName[propertyName];
        Assert.Equal(expected, actual);

        columnBase = NewColumn(new PropertyName(propertyName));
        actual = columnBase.ColumnInfo.ColumnTag.ColumnName;
        Assert.Equal(expected, actual);
    }

    protected void ValidateExpectedPropertyTableTag(string propertyName)
    {
        ColumnBase columnBase = NewColumn(propertyName);
        TableTag actual = columnBase.ColumnInfo.ColumnTag.TableTag;
        TableTag expected = ExpectedTableTag;
        Assert.Equal(expected, actual);

        columnBase = NewColumn(new PropertyName(propertyName));
        actual = columnBase.ColumnInfo.ColumnTag.TableTag;
        Assert.Equal(expected, actual);
    }

    protected void ValidateExpectedPropertyTableName(string propertyName)
    {
        ColumnBase columnBase = NewColumn(propertyName);
        TableName actual = columnBase.ColumnInfo.ColumnTag.TableTag.TableName;
        TableName expected = ExpectedTableName;
        Assert.Equal(expected, actual);

        columnBase = NewColumn(new PropertyName(propertyName));
        actual = columnBase.ColumnInfo.ColumnTag.TableTag.TableName;
        Assert.Equal(expected, actual);
    }

    protected void ValidateExpectedPropertySchemaName(string propertyName)
    {
        ColumnBase columnBase = NewColumn(propertyName);
        SchemaName? actual = columnBase.ColumnInfo.ColumnTag.TableTag.SchemaName;
        SchemaName? expected = ExpectedScehemaName;
        if (expected is null)
            Assert.Null(actual);
        else
            Assert.Equal(expected, actual);

        columnBase = NewColumn(new PropertyName(propertyName));
        actual = columnBase.ColumnInfo.ColumnTag.TableTag.SchemaName;
        if (expected is null)
            Assert.Null(actual);
        else
            Assert.Equal(expected, actual);
    }

    protected void ValidateNoDescendantParameters(string propertyName)
    {
        ColumnBase column = NewColumn(propertyName);
        int expectedValue = 0;
        int actual = column.DescendantParameters.Count();

        Assert.Equal(expectedValue, actual);

        column = NewColumn(new PropertyName(propertyName));
        actual = column.DescendantParameters.Count();

        Assert.Equal(expectedValue, actual);
    }

    protected void ValidateNoDescendantColumns(string propertyName)
    {
        ColumnBase column = NewColumn(propertyName);
        int expectedValue = 0;
        int actual = column.DescendantColumns.Count();

        Assert.Equal(expectedValue, actual);

        column = NewColumn(new PropertyName(propertyName));
        actual = column.DescendantColumns.Count();

        Assert.Equal(expectedValue, actual);
    }

    protected void RunValidationMethod(Action<string> action)
    {
        foreach (string propertyName in ExpectedPropertyColumnTag.Keys)
        {
            action(propertyName);
        }
    }

    protected void ValidateNotMapped()
    {
        foreach (string propertyName in NotMappedProperties)
        {
            Assert.Throws<InvalidPropertyException<modelT>>(() => NewColumn(propertyName));
            Assert.Throws<InvalidPropertyException<modelT>>(() => NewColumn(new PropertyName(propertyName)));
        }
    }

    [Fact]
    public void Constructor_ArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => NewColumn(null!));

    [Fact]
    public void Constructor_ArgumentNullException_FromPropertyName() =>
        Assert.Throws<ArgumentNullException>(() => NewColumn((PropertyName)null!));

    [Fact]
    public void Constructor_InvalidPropertyException_EmptyString() =>
        Assert.Throws<InvalidPropertyException<modelT>>(() => NewColumn(string.Empty));
    [Fact]
    public void Constructor_InvalidPropertyException_FromEmptyPropertyName() =>
        Assert.Throws<InvalidPropertyException<modelT>>(() => NewColumn(new PropertyName(string.Empty)));

    [Fact]
    public void Constructor_InvalidPropertyException_From_BadPropertyName() =>
        Assert.Throws<InvalidPropertyException<modelT>>(() => 
        NewColumn("C#"));

    [Fact]
    public void Constructor_InvalidPropertyException_FromBadPropertyName() =>
        Assert.Throws<InvalidPropertyException<modelT>>(() =>
        NewColumn(new PropertyName("C#")));

    [Fact]
    public void Run_ValidateNoDescendantParameters() =>
        RunValidationMethod(ValidateNoDescendantParameters);

    [Fact]
    public void Run_ValidateNoDescendantColumns() =>
        RunValidationMethod(ValidateNoDescendantColumns);

    [Fact]
    public void Run_ValidateSqlFragment() =>
        RunValidationMethod(ValidateSqlFragment);

    [Fact]
    public void Run_ValidateExpectedPropertyColumnTag() =>
        RunValidationMethod(ValidateExpectedPropertyColumnTag);

    [Fact]
    public void Run_ValidateExpectedPropertyColumnName() =>
        RunValidationMethod(ValidateExpectedPropertyColumnName);

    [Fact]
    public void Run_ValidateExpectedPropertyTableTag() =>
        RunValidationMethod(ValidateExpectedPropertyTableTag);

    [Fact]
    public void Run_ValidateExpectedPropertyTableName() =>
        RunValidationMethod(ValidateExpectedPropertyTableName);

    [Fact]
    public void Run_ValidateExpectedPropertySchemaName() =>
        RunValidationMethod(ValidateExpectedPropertySchemaName);

    [Fact]
    public void Run_ValidateNotMapped() =>
        ValidateNotMapped();
}
