using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class ParameterBaseTests<modelT> : ParameterTestsBase<modelT> where modelT : class
{
    protected abstract Parameter NewParameter(string propertyName, object? value);
    protected abstract Parameter NewParameter(PropertyName propertyName, object? value);
    protected abstract string ExpectSqlFragment(ParameterTag parameterTag);

    private void RunSubMethod(Action<Parameter> test, string propertyName)
    {
        object value = GetValue(propertyName);
        test(NewParameter(propertyName, value));
        test(NewParameter(new PropertyName(propertyName), value));
    }

    private void RunExceptionalTests(Func<Action, object?> exceptionTest, string? propertyName)
    {
        exceptionTest(() => NewParameter(propertyName!, 1));
        exceptionTest(() => NewParameter(new PropertyName(propertyName), 1));
    }

    protected override void ValidateSqlFragment(string propertyName)
    {
        void Test(Parameter parameter)
        {
            string actual = parameter.ToSqlFragments(Dialect).ToSql(Dialect);
            string expected = ExpectSqlFragment(ExpectedPropertyParameterTag[propertyName]);
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateExpectedPropertyParameterTag(string propertyName)
    {
        void Test(Parameter parameter)
        {
            ParameterTag actual = parameter.Name;
            ParameterTag expected = ExpectedPropertyParameterTag[propertyName];
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateFieldProperties(string propertyName)
    {
        void Test(Parameter parameter)
        {
            FieldProperties expected = GetExpectedFieldProperties(propertyName);
            AssertFieldProperties(expected, parameter.FieldProperties);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateValue(string propertyName)
    {
        void Test(Parameter parameter)
        {
            object expected = GetValue(propertyName);
            if (expected is byte[] expectedBytes)
                Assert.Equal(expectedBytes, Assert.IsType<byte[]>(parameter.Value));
            else
                Assert.Equal(expected, parameter.Value);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateNoDescendantParameters(string propertyName)
    {
        static void Test(Parameter parameter) =>
            Assert.Empty(parameter.DescendantParameters);
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateNoDescendantColumns(string propertyName)
    {
        static void Test(Parameter parameter) =>
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
