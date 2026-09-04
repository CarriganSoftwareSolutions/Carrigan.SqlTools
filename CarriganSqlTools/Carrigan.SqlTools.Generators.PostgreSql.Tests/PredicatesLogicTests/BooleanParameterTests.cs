using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.PredicatesLogicTests;

public class BooleanParameterTests
{
    protected static string ToExpectedParameterName(int expectedPosition) =>
        $"${expectedPosition}";

    protected static ISqlDialects Dialect => new PostgreSqlDialect();

    protected static Parameter NewParameterFromBooleanParameter(bool? value, FieldProperties? field = null)
    {
        BooleanParameter booleanParameter = new(value, field);
        Parameter parameter = booleanParameter;
        return parameter;
    }

    protected static Parameter NewParameterFromBooleanParameter(bool? value, string parameterName, FieldProperties? field = null)
    {
        BooleanParameter booleanParameter = new(value, new ParameterTag(parameterName), field);
        Parameter parameter = booleanParameter;
        return parameter;
    }

    protected static Parameter NewParameterFromBooleanParameter(bool? value, ParameterTag parameterTag, FieldProperties? field = null)
    {
        BooleanParameter booleanParameter = new(value, parameterTag, field);
        Parameter parameter = booleanParameter;
        return parameter;
    }

    #region Run exception tests

    protected static void RunExceptionTest(Func<Action, object?> assertExceptionTest, Func<bool?, string, FieldProperties?, Parameter> createParameter, string parameterName) =>
        assertExceptionTest(() => createParameter(true, parameterName, null));

    private static void RunExceptionTests(Func<Action, object?> exceptionTest, string parameterName) =>
        RunExceptionTest(exceptionTest, NewParameterFromBooleanParameter, parameterName);

    #endregion

    #region Run parameter tests

    protected static void ParameterTest(Parameter parameter, bool? expectedValue, string expectedParameterName)
    {
        Assert.Equal(expectedParameterName, parameter.ToSqlFragments(BooleanParameterTests.Dialect).ToSql(BooleanParameterTests.Dialect));
        Assert.Equal(expectedValue, parameter.Value);

        Assert.Empty(parameter.DescendantParameters);
        Assert.Empty(parameter.DescendantColumns);
        Assert.Empty(parameter.DescendantLeafTables);
        Assert.Empty(parameter.DescendantNodes);
        Assert.Empty(parameter.ChildNodes);
    }

    protected static void RunParameterTest(Action<Parameter, bool?, string> parameterTest, Func<bool?, string, FieldProperties?, Parameter> createParameter, bool? value, string parameterName)
    {
        Parameter parameter = createParameter(value, parameterName, null);
        parameterTest(parameter, value, BooleanParameterTests.ToExpectedParameterName(1));
    }

    protected static void RunParameterTest(Action<Parameter, bool?, string> parameterTest, Func<bool?, ParameterTag, FieldProperties?, Parameter> createParameter, bool? value, ParameterTag parameterTag)
    {
        Parameter parameter = createParameter(value, parameterTag, null);
        parameterTest(parameter, value, BooleanParameterTests.ToExpectedParameterName(1));
    }

    private static void RunParameterTests(string parameterName)
    {
        RunParameterTest(ParameterTest, NewParameterFromBooleanParameter, null, parameterName);
        RunParameterTest(ParameterTest, NewParameterFromBooleanParameter, false, parameterName);
        RunParameterTest(ParameterTest, NewParameterFromBooleanParameter, true, parameterName);
        RunParameterTest(ParameterTest, NewParameterFromBooleanParameter, null, new ParameterTag(parameterName));
        RunParameterTest(ParameterTest, NewParameterFromBooleanParameter, false, new ParameterTag(parameterName));
        RunParameterTest(ParameterTest, NewParameterFromBooleanParameter, true, new ParameterTag(parameterName));
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
            new Equal(new Column<SqlTypeEntity>("BoolValue"), new BooleanParameter(false, new ParameterTag("Test"))),
            new Equal(new Column<SqlTypeEntity>("BoolValue"), new BooleanParameter(true, new ParameterTag("Test"))),
            new Equal(new Column<SqlTypeEntity>("BoolValue"), new BooleanParameter(false, new ParameterTag("Test"))),
            new Equal(new Column<SqlTypeEntity>("BoolValue"), new BooleanParameter(true, new ParameterTag("Test"))),
            new Equal(new Column<SqlTypeEntity>("BoolValue"), new BooleanParameter(false, new ParameterTag("NotTest"))),
            new Equal(new Column<SqlTypeEntity>("BoolValue"), new BooleanParameter(true, new ParameterTag("Test")))
        );

        string expected = "((\"TestSqlTypes\".\"BoolValue\" = $1) OR (\"TestSqlTypes\".\"BoolValue\" = $2) OR (\"TestSqlTypes\".\"BoolValue\" = $3) OR " +
            "(\"TestSqlTypes\".\"BoolValue\" = $4) OR (\"TestSqlTypes\".\"BoolValue\" = $5) OR (\"TestSqlTypes\".\"BoolValue\" = $6))";

        string actual = predicate.ToSqlFragments(BooleanParameterTests.Dialect).ToSql(BooleanParameterTests.Dialect);
        Assert.Equal(expected, actual);

        bool actualBool;
        bool expectedBool;

        actualBool = (bool)predicate.ToSqlFragments(BooleanParameterTests.Dialect).GetSqlFragmentParameters(BooleanParameterTests.Dialect).Single(parameter => parameter.ParameterTag == "$1").Value!;
        expectedBool = false;
        Assert.Equal(expectedBool, actualBool);

        actualBool = (bool)predicate.ToSqlFragments(BooleanParameterTests.Dialect).GetSqlFragmentParameters(BooleanParameterTests.Dialect).Single(parameter => parameter.ParameterTag == "$2").Value!;
        expectedBool = true;
        Assert.Equal(expectedBool, actualBool);

        actualBool = (bool)predicate.ToSqlFragments(BooleanParameterTests.Dialect).GetSqlFragmentParameters(BooleanParameterTests.Dialect).Single(parameter => parameter.ParameterTag == "$3").Value!;
        expectedBool = false;
        Assert.Equal(expectedBool, actualBool);

        actualBool = (bool)predicate.ToSqlFragments(BooleanParameterTests.Dialect).GetSqlFragmentParameters(BooleanParameterTests.Dialect).Single(parameter => parameter.ParameterTag == "$4").Value!;
        expectedBool = true;
        Assert.Equal(expectedBool, actualBool);

        actualBool = (bool)predicate.ToSqlFragments(BooleanParameterTests.Dialect).GetSqlFragmentParameters(BooleanParameterTests.Dialect).Single(parameter => parameter.ParameterTag == "$5").Value!;
        expectedBool = false;
        Assert.Equal(expectedBool, actualBool);

        actualBool = (bool)predicate.ToSqlFragments(BooleanParameterTests.Dialect).GetSqlFragmentParameters(BooleanParameterTests.Dialect).Single(parameter => parameter.ParameterTag == "$6").Value!;
        expectedBool = true;
        Assert.Equal(expectedBool, actualBool);
    }
}