using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

public class NotTests
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
    public void Not_1_ToSql()
    {
        PredicateBase inner = ColumnTastyPizza;
        string innerSql = ColumnTastyPizzaExpectedSql;

        PredicateBase predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Not_1_ParameterCount()
    {
        PredicateBase inner = ColumnTastyPizza;

        PredicateBase predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_2_ToSql()
    {
        PredicateBase inner = ColumnDestructCode;
        string innerSql = ColumnDestructCodeSql;

        PredicateBase predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Not_2_ParameterCount()
    {
        PredicateBase inner = ColumnDestructCode;

        PredicateBase predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ToSql()
    {
        PredicateBase inner = ColumnFutureCity;
        string innerSql = ColumnFutureCitySql;

        PredicateBase predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ParameterCount()
    {
        PredicateBase inner = ColumnFutureCity;

        PredicateBase predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ToSql()
    {
        PredicateBase inner = ParameterPi;
        string innerSql = ParameterPiSql;

        PredicateBase predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ParameterCount()
    {
        PredicateBase inner = ParameterPi;

        PredicateBase predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ParameterValues()
    {
        PredicateBase inner = ParameterPi;

        PredicateBase predicate = new Not(inner);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.Parameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }


    [Fact]
    public void Not_5_ToSql()
    {
        PredicateBase inner = ParameterElite;
        string innerSql = ParameterEliteSql;

        PredicateBase predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ParameterCount()
    {
        PredicateBase inner = ParameterElite;

        PredicateBase predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ParameterValue()
    {
        PredicateBase inner = ParameterElite;

        PredicateBase predicate = new Not(inner);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.Parameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        Assert.Equal(expectedValueInt, actualValueInt);
    }






    [Fact]
    public void Not_6_ToSql()
    {
        PredicateBase inner = ParameterHelloWorld;
        string innerSql = ParameterHelloWorldSql;

        PredicateBase predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ParameterCount()
    {
        PredicateBase inner = ParameterHelloWorld;

        PredicateBase predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ParameterValue()
    {

        PredicateBase inner = ParameterHelloWorld;

        PredicateBase predicate = new Not(inner);

        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.Parameters.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Not_Nested_ToSql()
    {
        PredicateBase and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));
        string andSql = $"((NOT {ParameterEliteSql}) AND (NOT {ParameterHelloWorldSql}) AND (NOT {ColumnFutureCitySql}) AND (NOT {ColumnDestructCodeSql}))";

        string expectedValue = andSql;
        string actualValue = and.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ParameterCount()
    {
        PredicateBase and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ParameterValue()
    {
        PredicateBase and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        int expectedValueInt = 1337;
        object? nullableActualValueInt = and.Parameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)and.Parameters.First(p => p.Name == "HelloWorld").Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Not_1_ColumnCount()
    {
        PredicateBase inner = ColumnTastyPizza;

        PredicateBase predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_1_ColumnName()
    {
        PredicateBase inner = ColumnTastyPizza;

        PredicateBase predicate = new Not(inner);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Pizza]").Single();
    }

    [Fact]
    public void Not_2_ColumnCount()
    {
        PredicateBase inner = ColumnDestructCode;

        PredicateBase predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_2_ColumnName()
    {
        PredicateBase inner = ColumnDestructCode;

        PredicateBase predicate = new Not(inner);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void Not_3_ColumnCount()
    {
        PredicateBase inner = ColumnFutureCity;

        PredicateBase predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ColumnName()
    {
        PredicateBase inner = ColumnFutureCity;

        PredicateBase predicate = new Not(inner);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void Not_4_ColumnCount()
    {
        PredicateBase inner = ParameterPi;

        PredicateBase predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ColumnCount()
    {
        PredicateBase inner = ParameterElite;

        PredicateBase predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ColumnCount()
    {
        PredicateBase inner = ParameterHelloWorld;

        PredicateBase predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ColumnCount()
    {
        PredicateBase and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ColumnName()
    {
        PredicateBase and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        _ = and.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
        _ = and.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }
}
