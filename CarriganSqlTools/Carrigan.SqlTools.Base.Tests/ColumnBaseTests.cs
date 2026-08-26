using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Base.Tests;

public abstract class ColumnBaseTests<modelT> where modelT : class
{
    protected abstract ISqlDialects Dialect { get; }

    protected abstract string? SchemaName { get; }

    protected abstract string TableName { get; }

    protected abstract ColumnBase NewColumn(string propertyName);

    protected abstract ColumnBase NewColumn(PropertyName propertyName);

    protected SqlExpression NewColumnAsExpression(string propertyName) =>
        NewColumn(propertyName);
    protected SqlExpression NewColumnAsExpression(PropertyName propertyName) =>
        NewColumn(propertyName);



    protected abstract ColumnBase NewNumericColumnModelTypeToColumnModelType(string propertyName);
    protected abstract ColumnBase NewNumericColumnModelTypeToColumnModelType(PropertyName propertyName);

    protected abstract ColumnBase NewNumericColumnModelTypeToColumnBaseModelType(string propertyName);
    protected abstract ColumnBase NewNumericColumnModelTypeToColumnBaseModelType(PropertyName propertyName);

    protected abstract ColumnBase NewNumericColumnModelTypeToColumnBase(string propertyName);
    protected abstract ColumnBase NewNumericColumnModelTypeToColumnBase(PropertyName propertyName);

    protected abstract ColumnBase NewNumericColumnBaseModelTypeToColumnModelType(string propertyName);
    protected abstract ColumnBase NewNumericColumnBaseModelTypeToColumnModelType(PropertyName propertyName);


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

    protected abstract IEnumerable<string> NumericProperties { get; }

    protected TableTag ExpectedTableTag =>
        new(SchemaName, TableName);

    protected TableName ExpectedTableName =>
        new(TableName);

    protected SchemaName? ExpectedSchemaName =>
        IdentifierTypes.SchemaName.New(SchemaName);

    protected abstract string ExpectSqlFragment(string expectedColumnName);

    protected ColumnBaseTests()
    {
    }

    private void RunSubMethod(Action<ColumnBase> test, string propertyName)
    {
        test(NewColumn(propertyName));
        test(NewColumn(new PropertyName(propertyName)));
        if (NumericProperties.Contains(propertyName))
        {

            test(NewNumericColumnModelTypeToColumnModelType(propertyName));
            test(NewNumericColumnModelTypeToColumnModelType(new PropertyName(propertyName)));

            test(NewNumericColumnModelTypeToColumnBaseModelType(propertyName));
            test(NewNumericColumnModelTypeToColumnBaseModelType(new PropertyName(propertyName)));

            test(NewNumericColumnModelTypeToColumnBase(propertyName));
            test(NewNumericColumnModelTypeToColumnBase(new PropertyName(propertyName)));

            test(NewNumericColumnBaseModelTypeToColumnModelType(propertyName));
            test(NewNumericColumnBaseModelTypeToColumnModelType(new PropertyName(propertyName)));
        }
        else
        {
            Assert.Throws<NonNumericValueException>(() => NewNumericColumnModelTypeToColumnModelType(propertyName));
            Assert.Throws<NonNumericValueException>(() => NewNumericColumnModelTypeToColumnModelType(new PropertyName(propertyName)));

            Assert.Throws<NonNumericValueException>(() => NewNumericColumnModelTypeToColumnBaseModelType(propertyName));
            Assert.Throws<NonNumericValueException>(() => NewNumericColumnModelTypeToColumnBaseModelType(new PropertyName(propertyName)));

            Assert.Throws<NonNumericValueException>(() => NewNumericColumnModelTypeToColumnBase(propertyName));
            Assert.Throws<NonNumericValueException>(() => NewNumericColumnModelTypeToColumnBase(new PropertyName(propertyName)));

            Assert.Throws<NonNumericValueException>(() => NewNumericColumnBaseModelTypeToColumnModelType(propertyName));
            Assert.Throws<NonNumericValueException>(() => NewNumericColumnBaseModelTypeToColumnModelType(new PropertyName(propertyName)));
        }
    }
    private void RunExceptionalTests(Func<Action, object?> exceptionTest, string? propertyName)
    {
        exceptionTest(() => NewColumn(propertyName!));
        exceptionTest(() => NewColumn(new PropertyName(propertyName)));

        exceptionTest(() => NewNumericColumnModelTypeToColumnModelType(propertyName!));
        exceptionTest(() => NewNumericColumnModelTypeToColumnModelType(new PropertyName(propertyName)));

        exceptionTest(() => NewNumericColumnModelTypeToColumnBaseModelType(propertyName!));
        exceptionTest(() => NewNumericColumnModelTypeToColumnBaseModelType(new PropertyName(propertyName)));

        exceptionTest(() => NewNumericColumnModelTypeToColumnBase(propertyName!));
        exceptionTest(() => NewNumericColumnModelTypeToColumnBase(new PropertyName(propertyName)));

        exceptionTest(() => NewNumericColumnBaseModelTypeToColumnModelType(propertyName!));
        exceptionTest(() => NewNumericColumnBaseModelTypeToColumnModelType(new PropertyName(propertyName)));

    }


    protected void ValidateColumnSqlFragments(string propertyName)
    {
        void Test(ColumnBase columnBase)
        {
            string actual = columnBase?.ToSqlFragments(Dialect)?.ToSql(Dialect) ?? string.Empty;
            string expected = ExpectSqlFragment(ExpectedPropertyColumnName[propertyName]);

            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected void ValidateSqlFragment(string propertyName)
    {
        void Test(SqlExpression expression)
        {
            string actual = expression?.ToSqlFragments(Dialect)?.ToSql(Dialect) ?? string.Empty;
            string expected = ExpectSqlFragment(ExpectedPropertyColumnName[propertyName]);

            Assert.Equal(expected, actual);
        }
        Test(NewColumn(propertyName));
        Test(NewColumn(new PropertyName(propertyName)));
        Test(NewColumnAsExpression(propertyName));
        Test(NewColumnAsExpression(new PropertyName(propertyName)));
    }

    protected void ValidateExpectedPropertyColumnTag(string propertyName)
    {
        void Test(ColumnBase columnBase)
        {
            ColumnTag actual = columnBase.ColumnInfo.ColumnTag;
            ColumnTag expected = ExpectedPropertyColumnTag[propertyName];
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected void ValidateExpectedPropertyColumnName(string propertyName)
    {
        void Test(ColumnBase columnBase)
        {
            ColumnName actual = columnBase.ColumnInfo.ColumnTag.ColumnName;
            ColumnName expected = ExpectedPropertyColumnName[propertyName];
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected void ValidateExpectedPropertyTableTag(string propertyName)
    {
        void Test(ColumnBase columnBase)
        {
            TableTag actual = columnBase.ColumnInfo.ColumnTag.TableTag;
            TableTag expected = ExpectedTableTag;
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected void ValidateExpectedPropertyTableName(string propertyName)
    {
        void Test(ColumnBase columnBase)
        {
            TableName actual = columnBase.ColumnInfo.ColumnTag.TableTag.TableName;
            TableName expected = ExpectedTableName;
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected void ValidateExpectedPropertySchemaName(string propertyName)
    {
        void Test(ColumnBase columnBase)
        {
            SchemaName? actual = columnBase.ColumnInfo.ColumnTag.TableTag.SchemaName;
            SchemaName? expected = ExpectedSchemaName;
            if (expected is null)
                Assert.Null(actual);
            else
                Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected void ValidateNoDescendantParameters(string propertyName)
    {
        static void Test(ColumnBase columnBase)
        {
            int expectedValue = 0;
            int actual = columnBase.DescendantParameters.Count();

            Assert.Equal(expectedValue, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected void ValidateNoDescendantColumns(string propertyName)
    {
        static void Test(ColumnBase columnBase)
        {
            int expectedValue = 0;
            int actual = columnBase.DescendantColumns.Count();

            Assert.Equal(expectedValue, actual);
        }
        RunSubMethod(Test, propertyName);
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
            RunExceptionalTests(Assert.Throws<InvalidPropertyException<modelT>>, propertyName);
        }
    }

    [Fact]
    public void Constructor_ArgumentNullException() =>
            RunExceptionalTests(Assert.Throws<InvalidPropertyException<modelT>>, null);

    [Fact]
    public void Constructor_InvalidPropertyException_EmptyString() =>
            RunExceptionalTests(Assert.Throws<InvalidPropertyException<modelT>>, string.Empty);

    [Fact]
    public void Constructor_InvalidPropertyException_From_BadPropertyName() =>
            RunExceptionalTests(Assert.Throws<InvalidPropertyException<modelT>>, "C#");

    [Fact]
    public void Run_ValidateNoDescendantParameters() =>
        RunValidationMethod(ValidateNoDescendantParameters);

    [Fact]
    public void Run_ValidateNoDescendantColumns() =>
        RunValidationMethod(ValidateNoDescendantColumns);

    [Fact]
    public void Run_ValidateColumnSqlFragments() =>
        RunValidationMethod(ValidateColumnSqlFragments);
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
