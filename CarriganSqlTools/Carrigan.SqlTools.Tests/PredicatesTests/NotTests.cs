using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

public class NotTests
{
    private readonly Predicates.Predicates ColumnTastyPizza = new Column<ColumnTable>("Pizza");
    private readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private readonly Predicates.Predicates ColumnDestructCode = new Column<ColumnTable>("D000destruct0");
    private readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private readonly Predicates.Predicates ColumnFutureCity = new Column<ColumnTable>("Express");
    private readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private readonly Predicates.Predicates ParameterPi = new Parameter("Pi", 3.14f);
    private readonly string ParameterPiSql = "@Parameter_Pi";

    private readonly Predicates.Predicates ParameterElite = new Parameter("Elite", 1337);
    private readonly string ParameterEliteSql = "@Parameter_Elite";

    private readonly Predicates.Predicates ParameterHelloWorld = new Parameter("HelloWorld", "Hello World!");
    private readonly string ParameterHelloWorldSql = "@Parameter_HelloWorld";


    [Fact]
    public void Not_1_ToSql()
    {
        Predicates.Predicates inner = ColumnTastyPizza;
        string innerSql = ColumnTastyPizzaExpectedSql;

        Predicates.Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Not_1_ParameterCount()
    {
        Predicates.Predicates inner = ColumnTastyPizza;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_2_ToSql()
    {
        Predicates.Predicates inner = ColumnDestructCode;
        string innerSql = ColumnDestructCodeSql;

        Predicates.Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Not_2_ParameterCount()
    {
        Predicates.Predicates inner = ColumnDestructCode;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ToSql()
    {
        Predicates.Predicates inner = ColumnFutureCity;
        string innerSql = ColumnFutureCitySql;

        Predicates.Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ParameterCount()
    {
        Predicates.Predicates inner = ColumnFutureCity;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ToSql()
    {
        Predicates.Predicates inner = ParameterPi;
        string innerSql = ParameterPiSql;

        Predicates.Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ParameterCount()
    {
        Predicates.Predicates inner = ParameterPi;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ParameterValues()
    {
        Predicates.Predicates inner = ParameterPi;

        Predicates.Predicates predicate = new Not(inner);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.Parameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }


    [Fact]
    public void Not_5_ToSql()
    {
        Predicates.Predicates inner = ParameterElite;
        string innerSql = ParameterEliteSql;

        Predicates.Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ParameterCount()
    {
        Predicates.Predicates inner = ParameterElite;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ParameterValue()
    {
        Predicates.Predicates inner = ParameterElite;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.Parameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        Assert.Equal(expectedValueInt, actualValueInt);
    }






    [Fact]
    public void Not_6_ToSql()
    {
        Predicates.Predicates inner = ParameterHelloWorld;
        string innerSql = ParameterHelloWorldSql;

        Predicates.Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ParameterCount()
    {
        Predicates.Predicates inner = ParameterHelloWorld;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ParameterValue()
    {

        Predicates.Predicates inner = ParameterHelloWorld;

        Predicates.Predicates predicate = new Not(inner);

        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.Parameters.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Not_Nested_ToSql()
    {
        Predicates.Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));
        string andSql = $"((NOT {ParameterEliteSql}) AND (NOT {ParameterHelloWorldSql}) AND (NOT {ColumnFutureCitySql}) AND (NOT {ColumnDestructCodeSql}))";

        string expectedValue = andSql;
        string actualValue = and.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ParameterCount()
    {
        Predicates.Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ParameterValue()
    {
        Predicates.Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

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
        Predicates.Predicates inner = ColumnTastyPizza;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_1_ColumnName()
    {
        Predicates.Predicates inner = ColumnTastyPizza;

        Predicates.Predicates predicate = new Not(inner);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Pizza]").Single();
    }

    [Fact]
    public void Not_2_ColumnCount()
    {
        Predicates.Predicates inner = ColumnDestructCode;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_2_ColumnName()
    {
        Predicates.Predicates inner = ColumnDestructCode;

        Predicates.Predicates predicate = new Not(inner);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void Not_3_ColumnCount()
    {
        Predicates.Predicates inner = ColumnFutureCity;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ColumnName()
    {
        Predicates.Predicates inner = ColumnFutureCity;

        Predicates.Predicates predicate = new Not(inner);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void Not_4_ColumnCount()
    {
        Predicates.Predicates inner = ParameterPi;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ColumnCount()
    {
        Predicates.Predicates inner = ParameterElite;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ColumnCount()
    {
        Predicates.Predicates inner = ParameterHelloWorld;

        Predicates.Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ColumnCount()
    {
        Predicates.Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ColumnName()
    {
        Predicates.Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        _ = and.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
        _ = and.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }
}
