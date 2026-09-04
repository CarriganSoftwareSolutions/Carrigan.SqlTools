using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class BooleanParameterBaseTests<modelT> : ParameterTestsBase<modelT> where modelT : class
{
    protected abstract BooleanParameter NewBooleanParameter(string propertyName, bool? value);
    protected abstract BooleanParameter NewBooleanParameter(PropertyName propertyName, bool? value);
    protected abstract string ExpectSqlFragment(ParameterTag parameterTag);
    protected abstract IEnumerable<string> BooleanProperties { get; }

    private void RunSubMethod(Action<BooleanParameter> test, string propertyName)
    {
        if (BooleanProperties.Contains(propertyName))
        {
            test(NewBooleanParameter(propertyName, true));
            test(NewBooleanParameter(new PropertyName(propertyName), true));
        }
        else
        {
            Assert.Throws<NonBooleanValueException>(() => NewBooleanParameter(propertyName, true));
            Assert.Throws<NonBooleanValueException>(() => NewBooleanParameter(new PropertyName(propertyName), true));
        }
    }

    private void RunExceptionalTests(Func<Action, object?> exceptionTest, string? propertyName)
    {
        exceptionTest(() => NewBooleanParameter(propertyName!, true));
        exceptionTest(() => NewBooleanParameter(new PropertyName(propertyName), true));
    }

    protected override void ValidateSqlFragment(string propertyName)
    {
        void Test(BooleanParameter parameter)
        {
            string actual = parameter.ToSqlFragments(Dialect).ToSql(Dialect);
            string expected = ExpectSqlFragment(ExpectedPropertyParameterTag[propertyName]);
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateExpectedPropertyParameterTag(string propertyName)
    {
        void Test(BooleanParameter parameter)
        {
            ParameterTag actual = parameter.Name;
            ParameterTag expected = ExpectedPropertyParameterTag[propertyName];
            Assert.Equal(expected, actual);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateFieldProperties(string propertyName)
    {
        void Test(BooleanParameter parameter)
        {
            FieldProperties expected = GetExpectedFieldProperties(propertyName);
            AssertFieldProperties(expected, parameter.FieldProperties);
        }
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateValue(string propertyName)
    {
        static void Test(BooleanParameter parameter) =>
            Assert.True(Assert.IsType<bool>(parameter.Value));
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateNoDescendantParameters(string propertyName)
    {
        static void Test(BooleanParameter parameter) =>
            Assert.Empty(parameter.DescendantParameters);
        RunSubMethod(Test, propertyName);
    }

    protected override void ValidateNoDescendantColumns(string propertyName)
    {
        static void Test(BooleanParameter parameter) =>
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
