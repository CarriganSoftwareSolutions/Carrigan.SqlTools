using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

public class EqualsTests
{
    private readonly PredicatesLogic.Predicates ColumnTastyPizza = new Column<ColumnTable>("Pizza");
    private readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private readonly PredicatesLogic.Predicates ColumnDestructCode = new Column<ColumnTable>("D000destruct0");
    private readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private readonly PredicatesLogic.Predicates ColumnFutureCity = new Column<ColumnTable>("Express");
    private readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private readonly PredicatesLogic.Predicates ParameterPi = new Parameter("Pi", 3.14f);
    private readonly string ParameterPiSql = "@Parameter_Pi";

    private readonly PredicatesLogic.Predicates ParameterElite = new Parameter("Elite", 1337);
    private readonly string ParameterEliteSql = "@Parameter_Elite";

    private readonly PredicatesLogic.Predicates ParameterHelloWorld = new Parameter("HelloWorld", "Hello World!");
    private readonly string ParameterHelloWorldSql = "@Parameter_HelloWorld";


    [Fact]
    public void Equals_ToSql1()
    {
        PredicatesLogic.Predicates left = ColumnTastyPizza;
        string leftSql = ColumnTastyPizzaExpectedSql;

        PredicatesLogic.Predicates right = ColumnDestructCode;
        string rightSql = ColumnDestructCodeSql;

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        string expectedValue = $"({leftSql} = {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Equals1_ParameterCount()
    {
        PredicatesLogic.Predicates left = ColumnTastyPizza;

        PredicatesLogic.Predicates right = ColumnDestructCode;

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        int expectedValue = 0;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Equals1_ColumnCount()
    {
        PredicatesLogic.Predicates left = ColumnTastyPizza;

        PredicatesLogic.Predicates right = ColumnDestructCode;

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]

    public void Equals1_ColumnName()
    {
        PredicatesLogic.Predicates left = ColumnTastyPizza;

        PredicatesLogic.Predicates right = ColumnDestructCode;

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        _ = predicate.Columns.Where(col => col.ColumnInfo == $"[ColumnTable].[Pizza]").Single();
        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void Equals_ToSql2()
    {
        PredicatesLogic.Predicates left = ColumnFutureCity;
        string leftSql = ColumnFutureCitySql;

        PredicatesLogic.Predicates right = ParameterPi;
        string rightSql = ParameterPiSql;

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        string expectedValue = $"({leftSql} = {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals2_ParameterCount()
    {
        PredicatesLogic.Predicates left = ColumnFutureCity;

        PredicatesLogic.Predicates right = ParameterPi;

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        int expectedValue = 1;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals2_ParameterValues()
    {
        PredicatesLogic.Predicates left = ColumnFutureCity;

        PredicatesLogic.Predicates right = ParameterPi;

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        float expectedValue = 3.14f;

        object? nullableActualValue = predicate.Parameters.First().Value;
        Assert.NotNull(nullableActualValue);
        float actualValue = (float)nullableActualValue;

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Equals2_ColumnCount()
    {
        PredicatesLogic.Predicates left = ColumnFutureCity;

        PredicatesLogic.Predicates right = ParameterPi;

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        int expectedValue = 1;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]

    public void Equals2_ColumnName()
    {
        PredicatesLogic.Predicates left = ColumnFutureCity;

        PredicatesLogic.Predicates right = ParameterPi;

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void Equals_ToSql3()
    {
        PredicatesLogic.Predicates left = ParameterElite;
        string leftSql = ParameterEliteSql;

        PredicatesLogic.Predicates right = ParameterHelloWorld;
        string rightSql = ParameterHelloWorldSql;

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        string expectedValue = $"({leftSql} = {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals3_ParameterCount()
    {
        PredicatesLogic.Predicates left = ParameterElite;

        PredicatesLogic.Predicates right = ParameterHelloWorld;

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.Parameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.Parameters.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }
    [Fact]
    public void Equals3_ColumnCount()
    {
        PredicatesLogic.Predicates left = ParameterElite;

        PredicatesLogic.Predicates right = ParameterPi;

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        int expectedValue = 0;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals_Nested_ToSql()
    {
        PredicatesLogic.Predicates left = ParameterElite;
        string leftSql = ParameterEliteSql;

        PredicatesLogic.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);
        string rightSql = $"({ParameterHelloWorldSql} AND {ColumnFutureCitySql} AND {ColumnDestructCodeSql})";

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        string expectedValue = $"({leftSql} = {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals_Nested_ParameterCount()
    {
        PredicatesLogic.Predicates left = ParameterElite;

        PredicatesLogic.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals_Nested_ColumnCount()
    {
        PredicatesLogic.Predicates left = ParameterElite;

        PredicatesLogic.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals_Nested_ParameterValue()
    {
        PredicatesLogic.Predicates left = ParameterElite;

        PredicatesLogic.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        int expectedValueInt = 1337;

        object? nullableActualValueInt = predicate.Parameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;

        string expectedValueString = "Hello World!";
        string actualValueString = ((string?)predicate.Parameters.Where(p => p.Name == "HelloWorld").First().Value) ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Equals_Nested_ColumnName()
    {
        PredicatesLogic.Predicates left = ParameterElite;

        PredicatesLogic.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicatesLogic.Predicates predicate = new Equal(left, right);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }
}
