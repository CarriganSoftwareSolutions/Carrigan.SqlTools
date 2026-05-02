using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

public class XorThanTests
{
    private static readonly PredicatesLogic.Predicates ColumnTastyPizza = new Column<ColumnTable>("Pizza");
    private static readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private static readonly PredicatesLogic.Predicates ColumnDestructCode = new Column<ColumnTable>("D000destruct0");
    private static readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private static readonly PredicatesLogic.Predicates ColumnFutureCity = new Column<ColumnTable>("Express");
    private static readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private static readonly PredicatesLogic.Predicates ParameterPi = new Parameter("Pi", 3.14f);
    private static readonly string ParameterPiSql = "@Pi_1";

    private static readonly PredicatesLogic.Predicates ParameterElite = new Parameter("Elite", 1337);
    private static readonly string ParameterEliteSql = "@Elite_1";

    private static readonly PredicatesLogic.Predicates ParameterHelloWorld = new Parameter("HelloWorld", "Hello World!");
    private static readonly string ParameterHelloWorldSql = "@HelloWorld_2";


    [Fact]
    public void Xor_1_ToSql()
    {
        PredicatesLogic.Predicates left = ColumnTastyPizza;
        string leftSql = ColumnTastyPizzaExpectedSql;

        PredicatesLogic.Predicates right = ColumnDestructCode;
        string rightSql = ColumnDestructCodeSql;

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSqlFragments().ToSql(new SqlServerDialect());

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Xor_1_ParameterCount()
    {
        PredicatesLogic.Predicates left = ColumnTastyPizza;

        PredicatesLogic.Predicates right = ColumnDestructCode;

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ToSql()
    {
        PredicatesLogic.Predicates left = ColumnFutureCity;
        string leftSql = ColumnFutureCitySql;

        PredicatesLogic.Predicates right = ParameterPi;
        string rightSql = ParameterPiSql;

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSqlFragments().ToSql(new SqlServerDialect());

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ParameterCount()
    {
        PredicatesLogic.Predicates left = ColumnFutureCity;

        PredicatesLogic.Predicates right = ParameterPi;

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ParameterValues()
    {
        PredicatesLogic.Predicates left = ColumnFutureCity;

        PredicatesLogic.Predicates right = ParameterPi;

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.DescendantParameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_3_ToSql()
    {
        PredicatesLogic.Predicates left = ParameterElite;
        string leftSql = ParameterEliteSql;

        PredicatesLogic.Predicates right = ParameterHelloWorld;
        string rightSql = ParameterHelloWorldSql;

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSqlFragments().ToSql(new SqlServerDialect());

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_3_ParameterCount()
    {
        PredicatesLogic.Predicates left = ParameterElite;

        PredicatesLogic.Predicates right = ParameterHelloWorld;

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.DescendantParameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.DescendantParameters.First(p => p.Name == "HelloWorld").Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Xor_Nested_ToSql()
    {
        PredicatesLogic.Predicates left = ParameterElite;
        string leftSql = ParameterEliteSql;

        PredicatesLogic.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);
        string rightSql = $"({ParameterHelloWorldSql} AND {ColumnFutureCitySql} AND {ColumnDestructCodeSql})";

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSqlFragments().ToSql(new SqlServerDialect());

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ParameterCount()
    {
        PredicatesLogic.Predicates left = ParameterElite;

        PredicatesLogic.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ParameterValue()
    {
        PredicatesLogic.Predicates left = ParameterElite;

        PredicatesLogic.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.DescendantParameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.DescendantParameters.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Xor_1_ColumnCount()
    {
        PredicatesLogic.Predicates left = ColumnTastyPizza;

        PredicatesLogic.Predicates right = ColumnDestructCode;

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_1_ColumnName()
    {
        PredicatesLogic.Predicates left = ColumnTastyPizza;

        PredicatesLogic.Predicates right = ColumnDestructCode;

        PredicatesLogic.Predicates predicate = new Xor(left, right);


        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[Pizza]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void Xor_2_ColumnCount()
    {
        PredicatesLogic.Predicates left = ColumnFutureCity;

        PredicatesLogic.Predicates right = ParameterPi;

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ColumnName()
    {
        PredicatesLogic.Predicates left = ColumnFutureCity;

        PredicatesLogic.Predicates right = ParameterPi;

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void Xor_3_ColumnCount()
    {
        PredicatesLogic.Predicates left = ParameterElite;

        PredicatesLogic.Predicates right = ParameterPi;

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ColumnCount()
    {
        PredicatesLogic.Predicates left = ParameterElite;

        PredicatesLogic.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ColumnName()
    {
        PredicatesLogic.Predicates left = ParameterElite;

        PredicatesLogic.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicatesLogic.Predicates predicate = new Xor(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }
}