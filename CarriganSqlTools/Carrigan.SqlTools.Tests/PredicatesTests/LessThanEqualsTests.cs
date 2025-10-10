using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

public class LessThanEqualsTests
{
    private readonly PredicateBase ColumnTastyPizza = new Column<ColumnTable>("Pizza");
    private readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private readonly PredicateBase ColumnDestructCode = new Column<ColumnTable>("D000destruct0");
    private readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private readonly PredicateBase ColumnFutureCity = new Column<ColumnTable>("Express");
    private readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private readonly PredicateBase ParameterPi = new Parameter("Pi", 3.14f);
    private readonly string ParameterPiSql = "@Parameter_Pi";

    private readonly PredicateBase ParameterElite = new Parameter("Elite", 1337);
    private readonly string ParameterEliteSql = "@Parameter_Elite";

    private readonly PredicateBase ParameterHelloWorld = new Parameter("HelloWorld", "Hello World!");
    private readonly string ParameterHelloWorldSql = "@Parameter_HelloWorld";


    [Fact]
    public void LessThanEquals_1_ToSql()
    {
        PredicateBase left = ColumnTastyPizza;
        string leftSql = ColumnTastyPizzaExpectedSql;

        PredicateBase right = ColumnDestructCode;
        string rightSql = ColumnDestructCodeSql;

        PredicateBase predicate = new LessThanEqual(left, right);

        string expectedValue = $"({leftSql} <= {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void LessThanEquals_1_ParameterCount()
    {
        PredicateBase left = ColumnTastyPizza;

        PredicateBase right = ColumnDestructCode;

        PredicateBase predicate = new LessThanEqual(left, right);

        int expectedValue = 0;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void LessThanEquals_2_ToSql()
    {
        PredicateBase left = ColumnFutureCity;
        string leftSql = ColumnFutureCitySql;

        PredicateBase right = ParameterPi;
        string rightSql = ParameterPiSql;

        PredicateBase predicate = new LessThanEqual(left, right);

        string expectedValue = $"({leftSql} <= {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void LessThanEquals_2_ParameterCount()
    {
        PredicateBase left = ColumnFutureCity;

        PredicateBase right = ParameterPi;

        PredicateBase predicate = new LessThanEqual(left, right);

        int expectedValue = 1;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void LessThanEquals_2_ParameterValues()
    {
        PredicateBase left = ColumnFutureCity;

        PredicateBase right = ParameterPi;

        PredicateBase predicate = new LessThanEqual(left, right);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.Parameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void LessThanEquals_3_ToSql()
    {
        PredicateBase left = ParameterElite;
        string leftSql = ParameterEliteSql;

        PredicateBase right = ParameterHelloWorld;
        string rightSql = ParameterHelloWorldSql;

        PredicateBase predicate = new LessThanEqual(left, right);

        string expectedValue = $"({leftSql} <= {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void LessThanEquals_3_ParameterCount()
    {
        PredicateBase left = ParameterElite;

        PredicateBase right = ParameterHelloWorld;

        PredicateBase predicate = new LessThanEqual(left, right);

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
    public void LessThanEquals_Nested_ToSql()
    {
        PredicateBase left = ParameterElite;
        string leftSql = ParameterEliteSql;

        PredicateBase right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);
        string rightSql = $"({ParameterHelloWorldSql} AND {ColumnFutureCitySql} AND {ColumnDestructCodeSql})";

        PredicateBase predicate = new LessThanEqual(left, right);

        string expectedValue = $"({leftSql} <= {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void LessThanEquals_Nested_ParameterCount()
    {
        PredicateBase left = ParameterElite;

        PredicateBase right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicateBase predicate = new LessThanEqual(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void LessThanEquals_Nested_ParameterValue()
    {
        PredicateBase left = ParameterElite;

        PredicateBase right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicateBase predicate = new LessThanEqual(left, right);

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
    public void LessThanEquals_1_ColumnCount()
    {
        PredicateBase left = ColumnTastyPizza;

        PredicateBase right = ColumnDestructCode;

        PredicateBase predicate = new LessThanEqual(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void LessThanEquals_1_ColumnName()
    {
        PredicateBase left = ColumnTastyPizza;

        PredicateBase right = ColumnDestructCode;

        PredicateBase predicate = new LessThanEqual(left, right);


        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Pizza]").Single();
        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void LessThanEquals_2_ColumnCount()
    {
        PredicateBase left = ColumnFutureCity;

        PredicateBase right = ParameterPi;

        PredicateBase predicate = new LessThanEqual(left, right);

        int expectedValue = 1;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void LessThanEquals_2_ColumnName()
    {
        PredicateBase left = ColumnFutureCity;

        PredicateBase right = ParameterPi;

        PredicateBase predicate = new LessThanEqual(left, right);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void LessThanEquals_3_ColumnCount()
    {
        PredicateBase left = ParameterElite;

        PredicateBase right = ParameterPi;

        PredicateBase predicate = new LessThanEqual(left, right);

        int expectedValue = 0;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void LessThanEquals_Nested_ColumnCount()
    {
        PredicateBase left = ParameterElite;

        PredicateBase right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicateBase predicate = new LessThanEqual(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void LessThanEquals_Nested_ColumnName()
    {
        PredicateBase left = ParameterElite;

        PredicateBase right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicateBase predicate = new LessThanEqual(left, right);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }
}
