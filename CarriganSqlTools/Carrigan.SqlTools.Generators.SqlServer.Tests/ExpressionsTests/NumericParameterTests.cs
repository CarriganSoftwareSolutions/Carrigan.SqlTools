using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.Data;
using System.Numerics;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class NumericParameterTests
{
    protected static string ToExpectedParameterName(string parameterName, int expectedPosition) => $"@{parameterName}_{expectedPosition}";

    protected static ISqlDialects Dialect => new SqlServerDialect();

    protected static Parameter NewParameterFromNumericParameterType<T>(T? value, FieldProperties? field = null) where T : INumber<T>
    {
        NumericParameter<T> numericParameter = new(value, field);
        Parameter parameter = numericParameter;
        return parameter;
    }

    protected static Parameter NewParameterFromNumericParameterType<T>(T? value, string parameterName, FieldProperties? field = null) where T : INumber<T>
    {
        NumericParameter<T> numericParameter = new(value, parameterName, field);
        Parameter parameter = numericParameter;
        return parameter;
    }

    protected static Parameter NewParameterFromNumericParameterType<T>(T? value, ParameterTag parameterTag, FieldProperties? field = null) where T : INumber<T>
    {
        NumericParameter<T> numericParameter = new(value, parameterTag, field);
        Parameter parameter = numericParameter;
        return parameter;
    }

    protected static Parameter NewParameterFromNumericParameter<T>(T? value, FieldProperties? field = null) where T : INumber<T>
    {
        NumericParameter numericParameter = NumericParameter.New(value, field);
        Parameter parameter = numericParameter;
        return parameter;
    }

    protected static Parameter NewParameterFromNumericParameter<T>(T? value, string parameterName, FieldProperties? field = null) where T : INumber<T>
    {
        NumericParameter numericParameter = NumericParameter.New(value, parameterName, field);
        Parameter parameter = numericParameter;
        return parameter;
    }

    protected static Parameter NewParameterFromNumericParameter<T>(T? value, ParameterTag parameterTag, FieldProperties? field = null) where T : INumber<T>
    {
        NumericParameter numericParameter = NumericParameter.New(value, parameterTag, field);
        Parameter parameter = numericParameter;
        return parameter;
    }

    protected static object? GetValue<T>() where T : INumber<T> => typeof(T) switch
    {
        Type t when t == typeof(short) => (short)42,
        Type t when t == typeof(int) => int.MaxValue,
        Type t when t == typeof(long) => long.MaxValue,
        Type t when t == typeof(float) => 3.141f,
        Type t when t == typeof(double) => 1.618d,
        Type t when t == typeof(decimal) => 2.71828m,
        _ => throw new NotSupportedException($"Type '{typeof(T).FullName}' is not supported.")
    };

    #region Run exception tests

    protected static void RunExceptionTest<T>(Func<Action, object?> assertExceptionTest, Func<T?, string, FieldProperties?, Parameter> createParameter, string parameterName)
        where T : INumber<T> =>
        assertExceptionTest(() => createParameter((T?)GetValue<T>(), parameterName, null));

    private static void RunExceptionTests(Func<Action, object?> exceptionTest, string parameterName)
    {
        RunExceptionTest<decimal>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
        RunExceptionTest<short>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
        RunExceptionTest<int>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
        RunExceptionTest<long>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
        RunExceptionTest<float>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
        RunExceptionTest<double>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
    }

    #endregion

    #region Run parameter tests

    protected static void ParameterTest<T>(Parameter parameter, T? expectedValue, string expectedParameterName) where T : INumber<T>
    {
        Assert.Equal(expectedParameterName, parameter.ToSqlFragments(NumericParameterTests.Dialect).ToSql(NumericParameterTests.Dialect));
        Assert.Equal(expectedValue, parameter.Value);

        Assert.Empty(parameter.DescendantParameters);
        Assert.Empty(parameter.DescendantColumns);
        Assert.Empty(parameter.DescendantLeafTables);
        Assert.Empty(parameter.DescendantNodes);
        Assert.Empty(parameter.ChildNodes);
    }

    protected static void RunParameterTest<T>(Action<Parameter, T?, string> parameterTest, Func<T?, string, FieldProperties?, Parameter> createParameter, string parameterName)
        where T : INumber<T>
    {
        T? value = (T?)GetValue<T>();
        Parameter parameter = createParameter(value, parameterName, null);

        parameterTest(parameter, value, NumericParameterTests.ToExpectedParameterName(parameterName, 1));
    }

    protected static void RunParameterTest<T>(Action<Parameter, T?, string> parameterTest, Func<T?, ParameterTag, FieldProperties?, Parameter> createParameter, ParameterTag parameterTag)
        where T : INumber<T>
    {
        T? value = (T?)GetValue<T>();
        Parameter parameter = createParameter(value, parameterTag, null);

        parameterTest(parameter, value, NumericParameterTests.ToExpectedParameterName(parameterTag, 1));
    }

    private static void RunParameterTests(string parameterName)
    {
        RunParameterTest<short>(ParameterTest, NewParameterFromNumericParameterType, parameterName);
        RunParameterTest<int>(ParameterTest, NewParameterFromNumericParameterType, parameterName);
        RunParameterTest<long>(ParameterTest, NewParameterFromNumericParameterType, parameterName);
        RunParameterTest<float>(ParameterTest, NewParameterFromNumericParameterType, parameterName);
        RunParameterTest<double>(ParameterTest, NewParameterFromNumericParameterType, parameterName);
        RunParameterTest<decimal>(ParameterTest, NewParameterFromNumericParameterType, parameterName);

        RunParameterTest<short>(ParameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
        RunParameterTest<int>(ParameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
        RunParameterTest<long>(ParameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));

        RunParameterTest<float>(ParameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
        RunParameterTest<double>(ParameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
        RunParameterTest<decimal>(ParameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
    }

    #endregion

    [Theory]
    [InlineData("@")]
    [InlineData("!")]
    [InlineData("%")]
    [InlineData("^")]
    [InlineData("&")]
    [InlineData("*")]
    [InlineData("(")]
    [InlineData(")")]
    [InlineData("-")]
    [InlineData("+")]
    [InlineData("=")]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("[")]
    [InlineData("]")]
    [InlineData("\\")]
    [InlineData("|")]
    [InlineData(":")]
    [InlineData(";")]
    [InlineData("\"")]
    [InlineData("'")]
    [InlineData("<")]
    [InlineData(">")]
    [InlineData("?")]
    [InlineData("/")]
    [InlineData("~")]
    [InlineData("`")]
    [InlineData(",")]
    [InlineData(".")]
    [InlineData("")]
    [InlineData("hello world")]
    public void ParameterValues_Theory_InvalidParameterChars(string parameterName) =>
        RunExceptionTests(Assert.Throws<InvalidParameterIdentifierException>, parameterName);

    [Fact]
    public void ParameterValues_null() =>
        RunExceptionTests(Assert.Throws<InvalidParameterIdentifierException>, null!);

    [Theory]
    [InlineData("Test")]
    [InlineData("Pi")]
    [InlineData("HelloWorld")]
    [InlineData("123")]
    [InlineData("_1")]
    public void Single_Parameter_Theory(string parameterName) =>
        RunParameterTests(parameterName);

    [Fact]
    public void Parameter_Multiple_Same_Name()
    {
        Predicates predicate = new Or
        (
            new Equal(new Column<SqlTypeEntity>("IntValue"), new NumericParameter<int>(0, "Test")),
            new Equal(new Column<SqlTypeEntity>("IntValue"), new NumericParameter<int>(1, "Test")),
            new Equal(new Column<SqlTypeEntity>("IntValue"), new NumericParameter<int>(2, "Test")),
            new Equal(new Column<SqlTypeEntity>("IntValue"), new NumericParameter<int>(3, "Test")),
            new Equal(new Column<SqlTypeEntity>("IntValue"), new NumericParameter<int>(4, "NotTest")),
            new Equal(new Column<SqlTypeEntity>("IntValue"), new NumericParameter<int>(10, "Test"))
        );

        string expected = "(([TestSqlTypes].[IntValue] = @Test_1) OR ([TestSqlTypes].[IntValue] = @Test_2) OR ([TestSqlTypes].[IntValue] = @Test_3) OR " +
            "([TestSqlTypes].[IntValue] = @Test_4) OR ([TestSqlTypes].[IntValue] = @NotTest_5) OR ([TestSqlTypes].[IntValue] = @Test_6))";

        string actual = predicate.ToSqlFragments(NumericParameterTests.Dialect).ToSql(NumericParameterTests.Dialect);
        Assert.Equal(expected, actual);

        int actualInt;
        int expectedInt;

        actualInt = (int)predicate.ToSqlFragments(NumericParameterTests.Dialect).GetSqlFragmentParameters(NumericParameterTests.Dialect).Single(parameter => parameter.ParameterTag == "@Test_1").Value!;
        expectedInt = 0;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments(NumericParameterTests.Dialect).GetSqlFragmentParameters(NumericParameterTests.Dialect).Single(parameter => parameter.ParameterTag == "@Test_2").Value!;
        expectedInt = 1;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments(NumericParameterTests.Dialect).GetSqlFragmentParameters(NumericParameterTests.Dialect).Single(parameter => parameter.ParameterTag == "@Test_3").Value!;
        expectedInt = 2;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments(NumericParameterTests.Dialect).GetSqlFragmentParameters(NumericParameterTests.Dialect).Single(parameter => parameter.ParameterTag == "@Test_4").Value!;
        expectedInt = 3;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments(NumericParameterTests.Dialect).GetSqlFragmentParameters(NumericParameterTests.Dialect).Single(parameter => parameter.ParameterTag == "@NotTest_5").Value!;
        expectedInt = 4;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments(NumericParameterTests.Dialect).GetSqlFragmentParameters(NumericParameterTests.Dialect).Single(parameter => parameter.ParameterTag == "@Test_6").Value!;
        expectedInt = 10;
        Assert.Equal(expectedInt, actualInt);
    }
}