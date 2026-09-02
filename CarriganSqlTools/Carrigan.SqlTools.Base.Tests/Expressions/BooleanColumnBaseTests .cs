using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class BooleanColumnBaseTests<modelT> : ColumnTestsBase<modelT> where modelT : class
{
    protected abstract BooleanColumnBase<modelT> NewBooleanColumn(string propertyName);

    protected abstract BooleanColumnBase<modelT> NewBooleanColumn(PropertyName propertyName);

    protected abstract Predicates NewColumnAsBooleanExpression(string propertyName);
    protected abstract Predicates NewColumnAsBooleanExpression(PropertyName propertyName);

    protected abstract BooleanColumnBase<modelT> NewColumnAsBooleanColumnBase(string propertyName);
    protected abstract BooleanColumnBase<modelT> NewColumnAsBooleanColumnBase(PropertyName propertyName);

    protected abstract BooleanColumnBase<modelT> NewColumnAsBooleanColumn(string propertyName);
    protected abstract BooleanColumnBase<modelT> NewColumnAsBooleanColumn(PropertyName propertyName);

    protected abstract IEnumerable<string> BooleanProperties { get; }

    protected BooleanColumnBaseTests()
    {
    }

    private void RunSubMethod(Action<BooleanColumnBase<modelT>> test, string propertyName)
    {
        if (BooleanProperties.Contains(propertyName))
        {
            test(NewColumnAsBooleanColumnBase(propertyName));
            test(NewColumnAsBooleanColumnBase(new PropertyName(propertyName)));

            test(NewColumnAsBooleanColumn(propertyName));
            test(NewColumnAsBooleanColumn(new PropertyName(propertyName)));
        }
        else
        {
            Assert.Throws<NonBooleanValueException>(() => NewColumnAsBooleanExpression(propertyName));
            Assert.Throws<NonBooleanValueException>(() => NewColumnAsBooleanExpression(new PropertyName(propertyName)));

            Assert.Throws<NonBooleanValueException>(() => NewColumnAsBooleanColumnBase(propertyName));
            Assert.Throws<NonBooleanValueException>(() => NewColumnAsBooleanColumnBase(new PropertyName(propertyName)));

            Assert.Throws<NonBooleanValueException>(() => NewColumnAsBooleanColumn(propertyName));
            Assert.Throws<NonBooleanValueException>(() => NewColumnAsBooleanColumn(new PropertyName(propertyName)));
        }
    }
    private void RunExceptionalTests(Func<Action, object?> exceptionTest, string? propertyName)
    {
        exceptionTest(() => NewColumnAsBooleanColumnBase(propertyName!));
        exceptionTest(() => NewColumnAsBooleanColumnBase(new PropertyName(propertyName)));

        exceptionTest(() => NewColumnAsBooleanColumn(propertyName!));
        exceptionTest(() => NewColumnAsBooleanColumn(new PropertyName(propertyName)));
    }


    protected override void ValidateColumnSqlFragments(string propertyName)
    {
        void Test(BooleanColumnBase<modelT> columnBase)
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
        if (BooleanProperties.Contains(propertyName))
        {
            Test(NewBooleanColumn(propertyName));
            Test(NewBooleanColumn(new PropertyName(propertyName)));
        }
        else
        {
            RunExceptionalTests(Assert.Throws<NonBooleanValueException>, propertyName);
        }
    }

    protected override void ValidateExpectedPropertyColumnTag(string propertyName)
    {
        void Test(BooleanColumnBase<modelT> columnBase)
        {
            ColumnTag actual = columnBase.ColumnInfo.ColumnTag;
            ColumnTag expected = ExpectedPropertyColumnTag[propertyName];
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateExpectedPropertyColumnName(string propertyName)
    {
        void Test(BooleanColumnBase<modelT> columnBase)
        {
            ColumnName actual = columnBase.ColumnInfo.ColumnTag.ColumnName;
            ColumnName expected = ExpectedPropertyColumnName[propertyName];
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateExpectedPropertyTableTag(string propertyName)
    {
        void Test(BooleanColumnBase<modelT> columnBase)
        {
            TableTag actual = columnBase.ColumnInfo.ColumnTag.TableTag;
            TableTag expected = ExpectedTableTag;
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateExpectedPropertyTableName(string propertyName)
    {
        void Test(BooleanColumnBase<modelT> columnBase)
        {
            TableName actual = columnBase.ColumnInfo.ColumnTag.TableTag.TableName;
            TableName expected = ExpectedTableName;
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateExpectedPropertySchemaName(string propertyName)
    {
        void Test(BooleanColumnBase<modelT> columnBase)
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
        static void Test(BooleanColumnBase<modelT> columnBase)
        {
            int expectedValue = 0;
            int actual = columnBase.DescendantParameters.Count();

            Assert.Equal(expectedValue, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateNoDescendantColumns(string propertyName)
    {
        static void Test(BooleanColumnBase<modelT> columnBase)
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
