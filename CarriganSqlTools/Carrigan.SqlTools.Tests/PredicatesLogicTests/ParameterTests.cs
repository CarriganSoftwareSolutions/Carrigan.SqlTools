using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Types;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

public class ParameterTests
{
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
        Assert.Throws<InvalidParameterIdentifierException>(() => new Parameter(param, 1, SqlTypeDefinition.AsInt()));

    [Fact]
    public void ParameterValues_Fact_NullParameter() => 
        Assert.Throws<ArgumentNullException>(() => new Parameter(null!, 1));


    [Theory]
    [InlineData("Test", 1, "@Parameter_Test")]
    [InlineData("Pi", 3.14f, "@Parameter_Pi")]
    [InlineData("HelloWorld", "Hello World", "@Parameter_HelloWorld")]
    [InlineData("123", 1, "@Parameter_123")]
    [InlineData("_1", 1, "@Parameter__1")]
    public void ParameterValues_Theory_SqlValues(string parameter, object value, object expected)
    {
        Parameter parameterValue = new(parameter, value);
        string actual = parameterValue.ToSqlFragments("Parameter").ToSql();

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
        Parameter parameterValue = new(new ParameterTag(null, parameter, null, new(value)), value);
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
        Parameter parameterValue = new(parameter, value);
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
        Parameter parameterValue = new(new ParameterTag(null, parameter, null, new(value)), value);
        string expected = $"{parameter}";
        string actual = parameterValue.Name;

        Assert.Equal(expected, actual);
    }


    [Fact]
    public void Parameter_Multiple_Same_Name()
    {
        PredicatesLogic.Predicates predicate = new Or
            (
                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter("Test", 0)),
                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter("Test", 1)),
                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter("Test", 2)),
                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter("Test", 3)),
                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter("NotTest", 4)),
                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter("Test", 10))
            );

        string expected = "(([TestSqlTypes].[IntValue] = @Parameter_0_R_Test) OR ([TestSqlTypes].[IntValue] = @Parameter_1_R_Test) OR ([TestSqlTypes].[IntValue] = @Parameter_2_R_Test) OR ([TestSqlTypes].[IntValue] = @Parameter_3_R_Test) OR ([TestSqlTypes].[IntValue] = @Parameter_NotTest) OR ([TestSqlTypes].[IntValue] = @Parameter_5_R_Test))";
        string actual = predicate.ToSqlFragments("Parameter").ToSql();
        Assert.Equal(expected, actual);

        int actualInt = predicate.DescendantParameters.Where(parameter => parameter.Name == "Test").Count();
        int expectedInt = 5;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments("Parameter").GetParameters().Where(parameter => parameter.Key == "@Parameter_0_R_Test").Single().Value!;
        expectedInt = 0;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments("Parameter").GetParameters().Where(parameter => parameter.Key == "@Parameter_1_R_Test").Single().Value!;
        expectedInt = 1;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments("Parameter").GetParameters().Where(parameter => parameter.Key == "@Parameter_2_R_Test").Single().Value!;
        expectedInt = 2;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments("Parameter").GetParameters().Where(parameter => parameter.Key == "@Parameter_3_R_Test").Single().Value!;
        expectedInt = 3;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments("Parameter").GetParameters().Where(parameter => parameter.Key == "@Parameter_NotTest").Single().Value!;
        expectedInt = 4;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments("Parameter").GetParameters().Where(parameter => parameter.Key == "@Parameter_5_R_Test").Single().Value!;
        expectedInt = 10;
        Assert.Equal(expectedInt, actualInt);
    }
    [Fact]
    public void Parameter_Multiple_Same_Name_Complex()
    {
        PredicatesLogic.Predicates predicate = new Or
            (
                new And
                    (
                        new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter("Test", 0)),
                        new Equal(new Column<SqlTypeEntity>("CharValue"), new Parameter("Test", 'A'))
                    ),
                new And
                    (
                        new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter("Test", 1)),
                        new Equal(new Parameter("Test", 'B'), new Column<SqlTypeEntity>("CharValue"))
                    )
            );

        string expected = "((([TestSqlTypes].[IntValue] = @Parameter_0_0_R_Test) AND ([TestSqlTypes].[CharValue] = @Parameter_0_1_R_Test)) OR (([TestSqlTypes].[IntValue] = @Parameter_1_0_R_Test) AND (@Parameter_1_1_L_Test = [TestSqlTypes].[CharValue])))";
        string actual = predicate.ToSqlFragments("Parameter").ToSql();
        Assert.Equal(expected, actual);

        int actualInt = predicate.DescendantParameters.Where(parameter => parameter.Name == "Test").Count();
        int expectedInt = 4;
        Assert.Equal(expectedInt, actualInt);

        actualInt = (int)predicate.ToSqlFragments("Parameter").GetParameters().Where(parameter => parameter.Key == "@Parameter_0_0_R_Test").Single().Value!;
        expectedInt = 0;
        Assert.Equal(actualInt, expectedInt);

        char actualChar = (char)predicate.ToSqlFragments("Parameter").GetParameters().Where(parameter => parameter.Key == "@Parameter_0_1_R_Test").Single().Value!;
        char expectedChar = 'A';
        Assert.Equal(actualChar, expectedChar);

        actualInt = (int)predicate.ToSqlFragments("Parameter").GetParameters().Where(parameter => parameter.Key == "@Parameter_1_0_R_Test").Single().Value!;
        expectedInt = 1;
        Assert.Equal(expectedInt, actualInt);

        actualChar = (char)predicate.ToSqlFragments("Parameter").GetParameters().Where(parameter => parameter.Key == "@Parameter_1_1_L_Test").Single().Value!;
        expectedChar = 'B';
        Assert.Equal(actualChar, expectedChar);
    }

    [Fact]
    public void Constructor_ExplicitSqlType_Compatible()
    {
        string parameterName = "IntValue";
        int value = 1;
        SqlTypeDefinition sqlTypeDefinition = SqlTypeDefinition.AsInt();

        Parameter parameter = new(parameterName, value, sqlTypeDefinition);

        Assert.Equal(parameterName, parameter.Name);
        Assert.Equal(value, parameter.Value);
    }

    [Fact]
    public void Constructor_ExplicitSqlType_Incompatible_Exception()
    {
        string parameterName = "IntValue";
        string value = "NotAnInt";
        SqlTypeDefinition sqlTypeDefinition = SqlTypeDefinition.AsInt();

        Assert.Throws<SqlTypeMismatchException>
        (
            () => new Parameter(parameterName, value, sqlTypeDefinition)
        );
    }

    [Fact]
    public void Constructor_ExplicitSqlType_DoubleNotInt_Exception()
    {
        string parameterName = "IntValue";
        double value = double.Pi;
        SqlTypeDefinition sqlTypeDefinition = SqlTypeDefinition.AsInt();

        Assert.Throws<SqlTypeMismatchException>
        (
            () => new Parameter(parameterName, value, sqlTypeDefinition)
        );
    }

    [Fact]
    public void Constructor_ExplicitSqlType_NullValue()
    {
        string parameterName = "IntValue";
        object? value = null;
        SqlTypeDefinition sqlTypeDefinition = SqlTypeDefinition.AsInt();

        Parameter parameter = new(parameterName, value, sqlTypeDefinition);

        Assert.Equal(parameterName, parameter.Name);
        Assert.Null(parameter.Value);
    }

    [Fact]
    public void GetParameters_NullValue()
    {
        string parameterName = "Name";
        object? value = null;

        Parameter parameter = new(parameterName, value);

        KeyValuePair<ParameterTag, object> singleParameter =
            parameter.ToSqlFragments("Parameter").GetParameters().Single();

        string expectedKey = "@Parameter_Name";
        object expectedValue = DBNull.Value;

        Assert.Equal(expectedKey, singleParameter.Key);
        Assert.Equal(expectedValue, singleParameter.Value);
    }

    [Fact]
    public void ToSqlFragments_NullBranchName_ThrowsArgumentNullException()
    {
        Parameter parameter = new("Test", 1);
        Assert.Throws<ArgumentNullException>(() => parameter.ToSqlFragments(null!));
    }



    [Fact]
    public void GetParameter_Value_SetsSqlTypeOnClone()
    {
        ParameterTag original = new(null, "IntParameter", null, null);

        Parameter result = new(original, 42);

        // Original should remain without a SqlType
        Assert.Null(original.SqlType);

        // Returned key should be a different instance with a non-null SqlType
        Assert.NotSame(original, result.Value);
        Assert.NotNull(result.Name.SqlType);
        Assert.Equal(SqlDbType.Int, result.Name.SqlType.Type);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void GetParameter_ValueNullUsesDBNullAndKeepsSqlTypeNull()
    {
        ParameterTag tag = new(null, "ParameterName", null, null);
        
        Parameter result = new(tag, null);

        // Uses DBNull.Value for the parameter value
        Assert.Same(DBNull.Value, result.Value);

        Assert.Equal(tag, result.Name);

        Assert.NotNull(result.Name.SqlType);
        Assert.Equal(SqlDbType.Variant, result.Name.SqlType.Type);
    }

    [Fact]
    public void GetParameter_Value_DoesNotChangeExistingSqlTypeOnOriginal()
    {
        SqlTypeDefinition presetType = SqlTypeDefinition.AsInt();
        ParameterTag original = new(null, "IntParameter", null, presetType);

        Parameter result =  new(original, 123);

        // Original should keep its preset type
        Assert.Same(presetType, original.SqlType);

        // Clone should also carry the same SqlTypeDefinition instance
        Assert.Same(presetType, result.Name.SqlType);
        Assert.Equal(123, result.Value);
        Assert.NotNull(result.Name.SqlType);
        Assert.Equal(SqlDbType.Int, result.Name.SqlType.Type);
    }

    [Fact]
    public void GetParameter_XDocument_ConvertsToString()
    {
        ParameterTag tag = new(null, "XmlParam", null, null);
        XDocument document = new(new XElement("Root", new XElement("Child", "Value")));

        Parameter result = new(tag, document);

        Assert.IsType<string>(result.Value);
        Assert.Contains("<Root>", (string)result.Value);
    }

    [Fact]
    public void GetParameter_XmlDocument_ConvertsToOuterXml()
    {
        ParameterTag tag = new(null, "XmlParam", null, null);

        XmlDocument document = new();
        document.LoadXml("<Root><Child>Value</Child></Root>");

        Parameter result = new(tag, document);

        Assert.IsType<string>(result.Value);
        Assert.Equal("<Root><Child>Value</Child></Root>", (string)result.Value);
    }
}
