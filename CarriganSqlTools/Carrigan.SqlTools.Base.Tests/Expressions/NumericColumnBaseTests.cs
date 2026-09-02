using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class NumericColumnBaseTests<modelT> : ColumnTestsBase<modelT> where modelT : class
{
    protected abstract NumericColumnBase<modelT> NewNumericColumn(string propertyName);

    protected abstract NumericColumnBase<modelT> NewNumericColumn(PropertyName propertyName);

    protected abstract NumericExpression NewColumnAsNumericExpression(string propertyName);
    protected abstract NumericExpression NewColumnAsNumericExpression(PropertyName propertyName);

    protected abstract NumericColumnBase<modelT> NewColumnAsNumericColumnBase(string propertyName);
    protected abstract NumericColumnBase<modelT> NewColumnAsNumericColumnBase(PropertyName propertyName);

    protected abstract NumericColumnBase<modelT> NewColumnAsNumericColumn(string propertyName);
    protected abstract NumericColumnBase<modelT> NewColumnAsNumericColumn(PropertyName propertyName);

    protected abstract IEnumerable<string> NumericProperties { get; }

    protected NumericColumnBaseTests()
    {
    }

    private void RunSubMethod(Action<NumericColumnBase<modelT>> test, string propertyName)
    {
        if (NumericProperties.Contains(propertyName))
        {
            test(NewColumnAsNumericColumnBase(propertyName));
            test(NewColumnAsNumericColumnBase(new PropertyName(propertyName)));

            test(NewColumnAsNumericColumn(propertyName));
            test(NewColumnAsNumericColumn(new PropertyName(propertyName)));
        }
        else
        {
            Assert.Throws<NonNumericValueException>(() => NewColumnAsNumericExpression(propertyName));
            Assert.Throws<NonNumericValueException>(() => NewColumnAsNumericExpression(new PropertyName(propertyName)));

            Assert.Throws<NonNumericValueException>(() => NewColumnAsNumericColumnBase(propertyName));
            Assert.Throws<NonNumericValueException>(() => NewColumnAsNumericColumnBase(new PropertyName(propertyName)));

            Assert.Throws<NonNumericValueException>(() => NewColumnAsNumericColumn(propertyName));
            Assert.Throws<NonNumericValueException>(() => NewColumnAsNumericColumn(new PropertyName(propertyName)));
        }
    }
    private void RunExceptionalTests(Func<Action, object?> exceptionTest, string? propertyName)
    {
        exceptionTest(() => NewColumnAsNumericColumnBase(propertyName!));
        exceptionTest(() => NewColumnAsNumericColumnBase(new PropertyName(propertyName)));

        exceptionTest(() => NewColumnAsNumericColumn(propertyName!));
        exceptionTest(() => NewColumnAsNumericColumn(new PropertyName(propertyName)));
    }


    protected override void ValidateColumnSqlFragments(string propertyName)
    {
        void Test(NumericColumnBase<modelT> columnBase)
        {
            string actual = columnBase?.ToSqlFragments(Dialect)?.ToSql(Dialect) ?? string.Empty;
            string expected = ExpectSqlFragment(ExpectedPropertyColumnName[propertyName]);

            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateSqlFragment(string propertyName)
    {
        void Test(SqlExpression expression)
        {
            string actual = expression?.ToSqlFragments(Dialect)?.ToSql(Dialect) ?? string.Empty;
            string expected = ExpectSqlFragment(ExpectedPropertyColumnName[propertyName]);

            Assert.Equal(expected, actual);
        }
        if (NumericProperties.Contains(propertyName))
        {
            Test(NewNumericColumn(propertyName));
            Test(NewNumericColumn(new PropertyName(propertyName)));
        }
        else
        {
            RunExceptionalTests(Assert.Throws<NonNumericValueException>, propertyName);
        }
    }

    protected override void ValidateExpectedPropertyColumnTag(string propertyName)
    {
        void Test(NumericColumnBase<modelT> columnBase)
        {
            ColumnTag actual = columnBase.ColumnInfo.ColumnTag;
            ColumnTag expected = ExpectedPropertyColumnTag[propertyName];
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateExpectedPropertyColumnName(string propertyName)
    {
        void Test(NumericColumnBase<modelT> columnBase)
        {
            ColumnName actual = columnBase.ColumnInfo.ColumnTag.ColumnName;
            ColumnName expected = ExpectedPropertyColumnName[propertyName];
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateExpectedPropertyTableTag(string propertyName)
    {
        void Test(NumericColumnBase<modelT> columnBase)
        {
            TableTag actual = columnBase.ColumnInfo.ColumnTag.TableTag;
            TableTag expected = ExpectedTableTag;
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateExpectedPropertyTableName(string propertyName)
    {
        void Test(NumericColumnBase<modelT> columnBase)
        {
            TableName actual = columnBase.ColumnInfo.ColumnTag.TableTag.TableName;
            TableName expected = ExpectedTableName;
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateExpectedPropertySchemaName(string propertyName)
    {
        void Test(NumericColumnBase<modelT> columnBase)
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

    protected override void ValidateNoDescendantParameters(string propertyName)
    {
        static void Test(NumericColumnBase<modelT> columnBase)
        {
            int expectedValue = 0;
            int actual = columnBase.DescendantParameters.Count();

            Assert.Equal(expectedValue, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateNoDescendantColumns(string propertyName)
    {
        static void Test(NumericColumnBase<modelT> columnBase)
        {
            int expectedValue = 0;
            int actual = columnBase.DescendantColumns.Count();

            Assert.Equal(expectedValue, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateNotMapped()
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
