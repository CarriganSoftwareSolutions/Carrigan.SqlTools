using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class NumericParameterBaseTests<modelT> : ParameterTestsBase<modelT> where modelT : class
{
    protected abstract NumericParameter NewNumericParameter(string propertyName, object value);
    protected abstract NumericParameter NewNumericParameter(PropertyName propertyName, object value);
    protected abstract string ExpectSqlFragment(ParameterTag parameterTag);
    protected abstract IEnumerable<string> NumericProperties { get; }

    private void RunSubMethod(Action<NumericParameter> test, string propertyName)
    {
        if (NumericProperties.Contains(propertyName))
        {
            object value = GetValue(propertyName);
            test(NewNumericParameter(propertyName, value));
            test(NewNumericParameter(new PropertyName(propertyName), value));
        }
        else
        {
            Assert.Throws<NonNumericValueException>(() => NewNumericParameter(propertyName, 1));
            Assert.Throws<NonNumericValueException>(() => NewNumericParameter(new PropertyName(propertyName), 1));
        }
    }

    private void RunExceptionalTests(Func<Action, object?> exceptionTest, string? propertyName)
    {
        exceptionTest(() => NewNumericParameter(propertyName!, 1));
        exceptionTest(() => NewNumericParameter(new PropertyName(propertyName), 1));
    }

    protected override void ValidateSqlFragment(string propertyName)
    {
        void Test(NumericParameter parameter)
        {
            string actual = parameter.ToSqlFragments(Dialect).ToSql(Dialect);
            string expected = ExpectSqlFragment(ExpectedPropertyParameterTag[propertyName]);
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateExpectedPropertyParameterTag(string propertyName)
    {
        void Test(NumericParameter parameter)
        {
            ParameterTag actual = parameter.Name;
            ParameterTag expected = ExpectedPropertyParameterTag[propertyName];
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateFieldProperties(string propertyName)
    {
        void Test(NumericParameter parameter)
        {
            FieldProperties expected = GetExpectedFieldProperties(propertyName);
            AssertFieldProperties(expected, parameter.FieldProperties);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateValue(string propertyName)
    {
        void Test(NumericParameter parameter)
        {
            object expected = GetValue(propertyName);
            Assert.Equal(expected, parameter.Value);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateNoDescendantParameters(string propertyName)
    {
        static void Test(NumericParameter parameter) =>
            Assert.Empty(parameter.DescendantParameters);
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateNoDescendantColumns(string propertyName)
    {
        static void Test(NumericParameter parameter) =>
            Assert.Empty(parameter.DescendantColumns);
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateNotMapped()
    {
        foreach (string propertyName in NotMappedProperties)
            RunExceptionalTests(Assert.Throws<InvalidPropertyException<modelT>>, propertyName);
    }

    [Fact]
    public void Constructor_NullProperty_Exception() =>
        RunExceptionalTests(Assert.Throws<InvalidPropertyException<modelT>>, null);

    [Fact]
    public void Constructor_EmptyProperty_Exception() =>
        RunExceptionalTests(Assert.Throws<InvalidPropertyException<modelT>>, string.Empty);

    [Fact]
    public void Constructor_InvalidProperty_Exception() =>
        RunExceptionalTests(Assert.Throws<InvalidPropertyException<modelT>>, "C#");
}
