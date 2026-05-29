using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class ParameterTests
{
    private static readonly SqlServerDialect Dialect = new();

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
    [InlineData("\'")]
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
    public void ParameterValues_Theory_InvalidParameterChars(string param) =>
        Assert.Throws<InvalidParameterIdentifierException>(() => new Parameter(1, param));


    [Theory]
    [InlineData("Test", 1, "@Test_1")]
    [InlineData("Pi", 3.14f, "@Pi_1")]
    [InlineData("HelloWorld", "Hello World", "@HelloWorld_1")]
    [InlineData("123", 1, "@123_1")]
    [InlineData("_1", 1, "@_1_1")]
    public void ParameterValues_Theory_SqlValues(string parameter, object value, object expected)
    {
        Parameter parameterValue = new(value, parameter);
        string actual = parameterValue.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Test", 1)]
    [InlineData("Pi", 3.14f)]
    [InlineData("HelloWorld", "Hello World")]
    [InlineData("123", 1)]
    [InlineData("_1", 1)]
    public void ParameterValues_ParameterCount(string parameter, object value)
    {
        Parameter parameterValue = new(value, new ParameterTag(parameter));
        int expected = 0;
        int actual = parameterValue.DescendantParameters.Count();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Test", 1)]
    [InlineData("Pi", 3.14f)]
    [InlineData("HelloWorld", "Hello World")]
    [InlineData("123", 1)]
    [InlineData("_1", 1)]
    public void ParameterValues_Parameter_Value(string parameter, object value)
    {
        Parameter parameterValue = new(value, parameter);
        object? expected = value;
        object? actual = parameterValue.Value;

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Test", 1)]
    [InlineData("Pi", 3.14f)]
    [InlineData("HelloWorld", "Hello World")]
    [InlineData("123", 1)]
    [InlineData("_1", 1)]
    public void ParameterValues_Parameter_Name(string parameter, object value)
    {
        Parameter parameterValue = new(value, new ParameterTag(parameter));
        string expected = $"{parameter}";
        string actual = parameterValue.Name;

        Assert.Equal(expected, actual);
    }


    [Fact]
    public void Parameter_Multiple_Same_Name()
    {
        PredicatesLogic.Predicates predicate = new Or
            (
                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(0, "Test")),
                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(1, "Test")),
                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(2, "Test")),
                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(3, "Test")),
                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(4, "NotTest")),
                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(10, "Test"))
            );

        string expected = "(([TestSqlTypes].[IntValue] = @Test_1) OR ([TestSqlTypes].[IntValue] = @Test_2) OR ([TestSqlTypes].[IntValue] = @Test_3) OR ([TestSqlTypes].[IntValue] = @Test_4) OR ([TestSqlTypes].[IntValue] = @NotTest_5) OR ([TestSqlTypes].[IntValue] = @Test_6))";
        string actual = predicate.ToSqlFragments(Dialect).ToSql(Dialect);
        Assert.Equal(expected, actual);

        int actualInt = predicate.DescendantParameters.Where(parameter => parameter.Name == "Test").Count();
        int expectedInt = 5;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_1").Single().Value!;
        expectedInt = 0;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_2").Single().Value!;
        expectedInt = 1;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_3").Single().Value!;
        expectedInt = 2;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_4").Single().Value!;
        expectedInt = 3;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@NotTest_5").Single().Value!;
        expectedInt = 4;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_6").Single().Value!;
        expectedInt = 10;
        Assert.Equal(expectedInt, actualInt);
    }

    [Fact]
    public void Parameter_Multiple_Same_Name_Complex()
    {
        Predicates predicate = new Or
            (
                new And
                    (
                        new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(0, "Test")),
                        new Equal(new Column<SqlTypeEntity>("CharValue"), new Parameter('A', "Test"))
                    ),
                new And
                    (
                        new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(1, "Test")),
                        new Equal(new Parameter('B', "Test"), new Column<SqlTypeEntity>("CharValue"))
                    )
            );

        string expected = "((([TestSqlTypes].[IntValue] = @Test_1) AND ([TestSqlTypes].[CharValue] = @Test_2)) OR (([TestSqlTypes].[IntValue] = @Test_3) AND (@Test_4 = [TestSqlTypes].[CharValue])))";
        string actual = predicate.ToSqlFragments(Dialect).ToSql(Dialect);
        Assert.Equal(expected, actual);

        int actualInt = predicate.DescendantParameters.Where(parameter => parameter.Name == "Test").Count();
        int expectedInt = 4;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_1").Single().Value!;
        expectedInt = 0;
        Assert.Equal(actualInt, expectedInt);

        char actualChar = (char)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_2").Single().Value!;
        char expectedChar = 'A';
        Assert.Equal(actualChar, expectedChar);

        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_3").Single().Value!;
        expectedInt = 1;
        Assert.Equal(expectedInt, actualInt);

        actualChar = (char)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_4").Single().Value!;
        expectedChar = 'B';
        Assert.Equal(actualChar, expectedChar);
    }

    [Fact]
    public void Constructor_ExplicitSqlType_NullValue()
    {
        string parameterName = "IntValue";
        object? value = null;

        Parameter parameter = new(value, parameterName);

        Assert.Equal(parameterName, parameter.Name);
        Assert.Null(parameter.Value);
    }

    [Fact]
    public void GetParameters_NullValue()
    {
        string parameterName = "Name";
        object? value = null;

        Parameter parameter = new(value, parameterName);

        SqlFragmentParameter singleParameter =
            parameter.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Single();

        string expectedKey = "@Name_1";
        object expectedValue = null!;

        Assert.Equal(expectedKey, singleParameter.ParameterTag.ToString());
        Assert.Equal(expectedValue, singleParameter.Value);
    }
}