//using Carrigan.SqlTools.Base.Tests.TestEntities;
//using Carrigan.SqlTools.Dialects;
//using Carrigan.SqlTools.Exceptions;
//using Carrigan.SqlTools.Expressions;
//using Carrigan.SqlTools.Fragments;
//using Carrigan.SqlTools.IdentifierTypes;
//using Carrigan.SqlTools.PredicatesLogic;
//using Carrigan.SqlTools.Tags;
//using Carrigan.SqlTools.Types;
//using System.Data;

//namespace Carrigan.SqlTools.Base.Tests.Expressions;

//public abstract class ParameterTestsBase
//{
//    protected abstract string ToExpectedParameterName(string parameterName, int expectedPosition);

//    protected abstract ISqlDialects Dialect { get; }

//    protected Parameter NewParameter(object? value, string? parameterName = null) =>
//        new Parameter(value, parameterName);

//    protected abstract Parameter NewParameter(object? value, ParameterTag parameterName);

//    protected abstract Parameter NewParameterFromModel(string propertyName, object? value);
//    protected abstract Parameter NewParameterFromModel(PropertyName propertyName, object? value);

//    protected abstract Parameter NewParameterModelFromNumericParameterModel<T>(T? value, string propertyName);
//    protected abstract Parameter NewParameterModelFromNumericParameterModel<T>(T? value, PropertyName propertyName);

//    protected abstract Parameter NewParameterFromNumericParameterModel<T>(T? value, string propertyName);
//    protected abstract Parameter NewParameterFromNumericParameterModel<T>(T? value, PropertyName propertyName);

//    protected abstract Parameter NewParameterFromNumericParameterType<T>(T? value, FieldProperties? field = null);
//    protected abstract Parameter NewParameterFromNumericParameterType<T>(T? value, string parameterTag, FieldProperties? field = null);
//    protected abstract Parameter NewParameterFromNumericParameterType<T>(T? value, ParameterTag parameterTag, FieldProperties? field = null);
//    protected abstract Parameter NewParameterFromNumericParameter<T>(T? value, FieldProperties? field = null);
//    protected abstract Parameter NewParameterFromNumericParameter<T>(T? value, string parameterTag, FieldProperties? field = null);
//    protected abstract Parameter NewParameterFromNumericParameter<T>(T? value, ParameterTag parameterTag, FieldProperties? field = null);

//    protected object[] ValueTypes =>
//        [(short)42, int.MaxValue, long.MaxValue, 3.141f, 1.618d, 2.71828m, "one", true, false, 'e', Guid.Empty ];

//    protected object? GetValue<T>() => typeof(T) switch
//    {
//        Type t when t == typeof(short) => (short)42,
//        Type t when t == typeof(int) => int.MaxValue,
//        Type t when t == typeof(long) => long.MaxValue,
//        Type t when t == typeof(float) => 3.141f,
//        Type t when t == typeof(double) => 1.618d,
//        Type t when t == typeof(decimal) => 2.71828m,
//        Type t when t == typeof(string) => "one",
//        Type t when t == typeof(bool) => true,
//        Type t when t == typeof(char) => 'e',
//        Type t when t == typeof(Guid) => Guid.Empty,
//        _ => throw new NotSupportedException($"Type '{typeof(T).FullName}' is not supported.")
//    };

//    protected KeyValuePair<string, ParameterTag> NewKvp(string propertyName, string parameterName) =>
//        new(propertyName, new ParameterTag(parameterName));

//    protected KeyValuePair<string, ParameterTag> NewKvp(string propertyName) =>
//        new(propertyName, new ParameterTag(propertyName));

//    internal abstract Dictionary<string, ParameterTag> ExpectedParameterTag { get; }

//    protected virtual IEnumerable<string> NotMappedProperties => 
//        [];

//    #region run exception tests
//    protected void RunExceptionTest(Func<Action, object?> assertExceptionTest, Func<object?, string?, Parameter> createParameter, string parameterName)
//    {
//        foreach(object value in  ValueTypes)
//        {
//            assertExceptionTest(() => createParameter(value, parameterName));
//        }
//    }
//    protected void RunExceptionTest(Func<Action, object?> assertExceptionTest, Func<object?, ParameterTag, Parameter> createParameter, ParameterTag parameterName)
//    {
//        foreach (object value in ValueTypes)
//        {
//            assertExceptionTest(() => createParameter(value, parameterName));
//        }
//    }

//    protected void RunExceptionTest<T>(Func<Action, object?> assertExceptionTest, Func<T?, string, FieldProperties?, Parameter> createParameter, string parameterName) => 
//        assertExceptionTest(() => createParameter((T?)GetValue<T>(), parameterName, null));

//    protected void RunExceptionTest<T>(Func<Action, object?> assertExceptionTest, Func<T?, ParameterTag, FieldProperties?, Parameter> createParameter, ParameterTag parameterName) =>
//        assertExceptionTest(() => createParameter((T?)GetValue<T>(), parameterName, null));
//    private void RunExceptionTests(Func<Action, object?> exceptionTest, string parameterName)
//    {
//        RunExceptionTest(exceptionTest, NewParameter, parameterName);
//        RunExceptionTest(exceptionTest, NewParameter, new ParameterTag(parameterName));

//        RunExceptionTest<short>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
//        RunExceptionTest<short>(exceptionTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunExceptionTest<int>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
//        RunExceptionTest<int>(exceptionTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunExceptionTest<long>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
//        RunExceptionTest<long>(exceptionTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));

//        RunExceptionTest<float>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
//        RunExceptionTest<float>(exceptionTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunExceptionTest<double>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
//        RunExceptionTest<double>(exceptionTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunExceptionTest<decimal>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
//        RunExceptionTest<decimal>(exceptionTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));


//        RunExceptionTest<string>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
//        RunExceptionTest<string>(exceptionTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunExceptionTest<bool>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
//        RunExceptionTest<bool>(exceptionTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunExceptionTest<char>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
//        RunExceptionTest<char>(exceptionTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunExceptionTest<Guid>(exceptionTest, NewParameterFromNumericParameterType, parameterName);
//        RunExceptionTest<Guid>(exceptionTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//    }
//    #endregion

//    #region run parameter tests


//    protected void RunParameterTest(Action<Parameter, string> parameterTest, Func<object?, string?, Parameter> createParameter, string parameterName)
//    {
//        foreach (object value in ValueTypes)
//        {
//            parameterTest(createParameter(value, parameterName), ToExpectedParameterName(parameterName));
//        }
//    }
//    protected void RunParameterTest(Action<Parameter, string> parameterTest, Func<object?, ParameterTag, Parameter> createParameter, ParameterTag parameterName)
//    {
//        foreach (object value in ValueTypes)
//        {
//            parameterTest(createParameter(value, parameterName));
//        }
//    }

//    protected void RunParameterTest<T>(Action<Parameter, string> parameterTest, Func<T?, string, FieldProperties?, Parameter> createParameter, string parameterName) =>
//        parameterTest(createParameter((T?)GetValue<T>(), parameterName, null));

//    protected void RunParameterTest<T>(Action<Parameter, string> parameterTest, Func<T?, ParameterTag, FieldProperties?, Parameter> createParameter, ParameterTag parameterName) =>
//        parameterTest(createParameter((T?)GetValue<T>(), parameterName, null));
//    private void RunParameterTests(Action<Parameter, string> parameterTest, string parameterName)
//    {
//        RunParameterTest(parameterTest, NewParameter, parameterName);
//        RunParameterTest(parameterTest, NewParameter, new ParameterTag(parameterName));

//        RunParameterTest<short>(parameterTest, NewParameterFromNumericParameterType, parameterName);
//        RunParameterTest<short>(parameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunParameterTest<int>(parameterTest, NewParameterFromNumericParameterType, parameterName);
//        RunParameterTest<int>(parameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunParameterTest<long>(parameterTest, NewParameterFromNumericParameterType, parameterName);
//        RunParameterTest<long>(parameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));

//        RunParameterTest<float>(parameterTest, NewParameterFromNumericParameterType, parameterName);
//        RunParameterTest<float>(parameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunParameterTest<double>(parameterTest, NewParameterFromNumericParameterType, parameterName);
//        RunParameterTest<double>(parameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunParameterTest<decimal>(parameterTest, NewParameterFromNumericParameterType, parameterName);
//        RunParameterTest<decimal>(parameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));


//        RunParameterTest<string>(parameterTest, NewParameterFromNumericParameterType, parameterName);
//        RunParameterTest<string>(parameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunParameterTest<bool>(parameterTest, NewParameterFromNumericParameterType, parameterName);
//        RunParameterTest<bool>(parameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunParameterTest<char>(parameterTest, NewParameterFromNumericParameterType, parameterName);
//        RunParameterTest<char>(parameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//        RunParameterTest<Guid>(parameterTest, NewParameterFromNumericParameterType, parameterName);
//        RunParameterTest<Guid>(parameterTest, NewParameterFromNumericParameterType, new ParameterTag(parameterName));
//    }
//    #endregion




//    [Theory]
//    [InlineData("@")]
//    [InlineData("!")]
//    [InlineData("%")]
//    [InlineData("^")]
//    [InlineData("&")]
//    [InlineData("*")]
//    [InlineData("(")]
//    [InlineData(")")]
//    [InlineData("-")]
//    [InlineData("+")]
//    [InlineData("=")]
//    [InlineData("{")]
//    [InlineData("}")]
//    [InlineData("[")]
//    [InlineData("]")]
//    [InlineData("\\")]
//    [InlineData("|")]
//    [InlineData(":")]
//    [InlineData(";")]
//    [InlineData("\"")]
//    [InlineData("\'")]
//    [InlineData("<")]
//    [InlineData(">")]
//    [InlineData("?")]
//    [InlineData("/")]
//    [InlineData("~")]
//    [InlineData("`")]
//    [InlineData(",")]
//    [InlineData(".")]
//    [InlineData("")]
//    [InlineData("hello world")]
//    public void ParameterValues_Theory_InvalidParameterChars(string parameterName) =>
//        RunExceptionTests(Assert.Throws<InvalidParameterIdentifierException>, parameterName);

//    [Fact]
//    public void ParameterValues_null() =>
//        RunExceptionTests(Assert.Throws<NullReferenceException>, (string)null!);

//    [Theory]
//    [InlineData("Test", 1, "@Test_1")]
//    [InlineData("Pi", 3.14f, "@Pi_1")]
//    [InlineData("HelloWorld", "Hello World", "@HelloWorld_1")]
//    [InlineData("123", 1, "@123_1")]
//    [InlineData("_1", 1, "@_1_1")]
//    public void ParameterValues_Theory_SqlValues(string parameterName, string expected)
//    {
//        RunParameterTests
//            ((Parameter parameter, string expectedParameterName) =>
//            {
//                string actual = parameter.ToSqlFragments(Dialect).ToSql(Dialect);

//                Assert.Equal(expected, actual);
//            }, parameterName


//            );
//    }

//    [Theory]
//    [InlineData("Test", 1)]
//    [InlineData("Pi", 3.14f)]
//    [InlineData("HelloWorld", "Hello World")]
//    [InlineData("123", 1)]
//    [InlineData("_1", 1)]
//    public void ParameterValues_ParameterCount(string parameter, object value)
//    {
//        Parameter parameterValue = new(value, new ParameterTag(parameter));
//        int expected = 0;
//        int actual = parameterValue.DescendantParameters.Count();

//        Assert.Equal(expected, actual);
//    }

//    [Theory]
//    [InlineData("Test", 1)]
//    [InlineData("Pi", 3.14f)]
//    [InlineData("HelloWorld", "Hello World")]
//    [InlineData("123", 1)]
//    [InlineData("_1", 1)]
//    public void ParameterValues_Parameter_Value(string parameter, object value)
//    {
//        Parameter parameterValue = new(value, parameter);
//        object? expected = value;
//        object? actual = parameterValue.Value;

//        Assert.Equal(expected, actual);
//    }

//    [Theory]
//    [InlineData("Test", 1)]
//    [InlineData("Pi", 3.14f)]
//    [InlineData("HelloWorld", "Hello World")]
//    [InlineData("123", 1)]
//    [InlineData("_1", 1)]
//    public void ParameterValues_Parameter_Name(string parameter, object value)
//    {
//        Parameter parameterValue = new(value, new ParameterTag(parameter));
//        string expected = $"{parameter}";
//        string actual = parameterValue.Name;

//        Assert.Equal(expected, actual);
//    }


//    [Fact]
//    public void Parameter_Multiple_Same_Name()
//    {
//        Predicates predicate = new Or
//            (
//                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(0, "Test")),
//                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(1, "Test")),
//                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(2, "Test")),
//                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(3, "Test")),
//                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(4, "NotTest")),
//                new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(10, "Test"))
//            );

//        string expected = "(([TestSqlTypes].[IntValue] = @Test_1) OR ([TestSqlTypes].[IntValue] = @Test_2) OR ([TestSqlTypes].[IntValue] = @Test_3) OR ([TestSqlTypes].[IntValue] = @Test_4) OR ([TestSqlTypes].[IntValue] = @NotTest_5) OR ([TestSqlTypes].[IntValue] = @Test_6))";
//        string actual = predicate.ToSqlFragments(Dialect).ToSql(Dialect);
//        Assert.Equal(expected, actual);

//        int actualInt = predicate.DescendantParameters.Where(parameter => parameter.Name == "Test").Count();
//        int expectedInt = 5;
//        Assert.Equal(expectedInt, actualInt);

//        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_1").Single().Value!;
//        expectedInt = 0;
//        Assert.Equal(expectedInt, actualInt);

//        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_2").Single().Value!;
//        expectedInt = 1;
//        Assert.Equal(expectedInt, actualInt);

//        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_3").Single().Value!;
//        expectedInt = 2;
//        Assert.Equal(expectedInt, actualInt);

//        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_4").Single().Value!;
//        expectedInt = 3;
//        Assert.Equal(expectedInt, actualInt);

//        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@NotTest_5").Single().Value!;
//        expectedInt = 4;
//        Assert.Equal(expectedInt, actualInt);

//        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_6").Single().Value!;
//        expectedInt = 10;
//        Assert.Equal(expectedInt, actualInt);
//    }

//    [Fact]
//    public void Parameter_Multiple_Same_Name_Complex()
//    {
//        Predicates predicate = new Or
//            (
//                new And
//                    (
//                        new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(0, "Test")),
//                        new Equal(new Column<SqlTypeEntity>("CharValue"), new Parameter('A', "Test"))
//                    ),
//                new And
//                    (
//                        new Equal(new Column<SqlTypeEntity>("IntValue"), new Parameter(1, "Test")),
//                        new Equal(new Parameter('B', "Test"), new Column<SqlTypeEntity>("CharValue"))
//                    )
//            );

//        string expected = "((([TestSqlTypes].[IntValue] = @Test_1) AND ([TestSqlTypes].[CharValue] = @Test_2)) OR (([TestSqlTypes].[IntValue] = @Test_3) AND (@Test_4 = [TestSqlTypes].[CharValue])))";
//        string actual = predicate.ToSqlFragments(Dialect).ToSql(Dialect);
//        Assert.Equal(expected, actual);

//        int actualInt = predicate.DescendantParameters.Where(parameter => parameter.Name == "Test").Count();
//        int expectedInt = 4;
//        Assert.Equal(expectedInt, actualInt);

//        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_1").Single().Value!;
//        expectedInt = 0;
//        Assert.Equal(actualInt, expectedInt);

//        char actualChar = (char)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_2").Single().Value!;
//        char expectedChar = 'A';
//        Assert.Equal(actualChar, expectedChar);

//        actualInt = (int)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_3").Single().Value!;
//        expectedInt = 1;
//        Assert.Equal(expectedInt, actualInt);

//        actualChar = (char)predicate.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Where(parameter => parameter.ParameterTag == "@Test_4").Single().Value!;
//        expectedChar = 'B';
//        Assert.Equal(actualChar, expectedChar);
//    }

//    [Fact]
//    public void Constructor_ExplicitSqlType_NullValue()
//    {
//        string parameterName = "IntValue";
//        object? value = null;

//        Parameter parameter = new(value, parameterName);

//        Assert.Equal(parameterName, parameter.Name);
//        Assert.Null(parameter.Value);
//    }

//    [Fact]
//    public void GetParameters_NullValue()
//    {
//        string parameterName = "Name";
//        object? value = null;

//        Parameter parameter = new(value, parameterName);

//        SqlFragmentParameter singleParameter =
//            parameter.ToSqlFragments(Dialect).GetSqlFragmentParameters(Dialect).Single();

//        string expectedKey = "@Name_1";
//        object expectedValue = null!;

//        Assert.Equal(expectedKey, singleParameter.ParameterTag.ToString());
//        Assert.Equal(expectedValue, singleParameter.Value);
//    }
//}