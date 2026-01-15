using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

public class NotTests
{
    private readonly PredicatesLogic.Predicates ColumnTastyPizza = new Column<ColumnTable>("Pizza");
    private readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private readonly PredicatesLogic.Predicates ColumnDestructCode = new Column<ColumnTable>("D000destruct0");
    private readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private readonly PredicatesLogic.Predicates ColumnFutureCity = new Column<ColumnTable>("Express");
    private readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private readonly PredicatesLogic.Predicates ParameterPi = new Parameter("Pi", 3.14f, null);
    private readonly string ParameterPiSql = "@Parameter_Pi";

    private readonly PredicatesLogic.Predicates ParameterElite = new Parameter("Elite", 1337, null);
    private readonly string ParameterEliteSql = "@Parameter_Elite";

    private readonly PredicatesLogic.Predicates ParameterHelloWorld = new Parameter("HelloWorld", "Hello World!", null);
    private readonly string ParameterHelloWorldSql = "@Parameter_HelloWorld";


    [Fact]
    public void Not_1_ToSql()
    {
        PredicatesLogic.Predicates inner = ColumnTastyPizza;
        string innerSql = ColumnTastyPizzaExpectedSql;

        PredicatesLogic.Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Not_1_ParameterCount()
    {
        PredicatesLogic.Predicates inner = ColumnTastyPizza;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_2_ToSql()
    {
        PredicatesLogic.Predicates inner = ColumnDestructCode;
        string innerSql = ColumnDestructCodeSql;

        PredicatesLogic.Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Not_2_ParameterCount()
    {
        PredicatesLogic.Predicates inner = ColumnDestructCode;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ToSql()
    {
        PredicatesLogic.Predicates inner = ColumnFutureCity;
        string innerSql = ColumnFutureCitySql;

        PredicatesLogic.Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ParameterCount()
    {
        PredicatesLogic.Predicates inner = ColumnFutureCity;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ToSql()
    {
        PredicatesLogic.Predicates inner = ParameterPi;
        string innerSql = ParameterPiSql;

        PredicatesLogic.Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ParameterCount()
    {
        PredicatesLogic.Predicates inner = ParameterPi;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ParameterValues()
    {
        PredicatesLogic.Predicates inner = ParameterPi;

        PredicatesLogic.Predicates predicate = new Not(inner);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.DescendantParameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }


    [Fact]
    public void Not_5_ToSql()
    {
        PredicatesLogic.Predicates inner = ParameterElite;
        string innerSql = ParameterEliteSql;

        PredicatesLogic.Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ParameterCount()
    {
        PredicatesLogic.Predicates inner = ParameterElite;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ParameterValue()
    {
        PredicatesLogic.Predicates inner = ParameterElite;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.DescendantParameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        Assert.Equal(expectedValueInt, actualValueInt);
    }






    [Fact]
    public void Not_6_ToSql()
    {
        PredicatesLogic.Predicates inner = ParameterHelloWorld;
        string innerSql = ParameterHelloWorldSql;

        PredicatesLogic.Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ParameterCount()
    {
        PredicatesLogic.Predicates inner = ParameterHelloWorld;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ParameterValue()
    {

        PredicatesLogic.Predicates inner = ParameterHelloWorld;

        PredicatesLogic.Predicates predicate = new Not(inner);

        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.DescendantParameters.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Not_Nested_ToSql()
    {
        PredicatesLogic.Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));
        string andSql = $"((NOT {ParameterEliteSql}) AND (NOT {ParameterHelloWorldSql}) AND (NOT {ColumnFutureCitySql}) AND (NOT {ColumnDestructCodeSql}))";

        string expectedValue = andSql;
        string actualValue = and.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ParameterCount()
    {
        PredicatesLogic.Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ParameterValue()
    {
        PredicatesLogic.Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        int expectedValueInt = 1337;
        object? nullableActualValueInt = and.DescendantParameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)and.DescendantParameters.First(p => p.Name == "HelloWorld").Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Not_1_ColumnCount()
    {
        PredicatesLogic.Predicates inner = ColumnTastyPizza;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_1_ColumnName()
    {
        PredicatesLogic.Predicates inner = ColumnTastyPizza;

        PredicatesLogic.Predicates predicate = new Not(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[Pizza]").Single();
    }

    [Fact]
    public void Not_2_ColumnCount()
    {
        PredicatesLogic.Predicates inner = ColumnDestructCode;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_2_ColumnName()
    {
        PredicatesLogic.Predicates inner = ColumnDestructCode;

        PredicatesLogic.Predicates predicate = new Not(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void Not_3_ColumnCount()
    {
        PredicatesLogic.Predicates inner = ColumnFutureCity;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ColumnName()
    {
        PredicatesLogic.Predicates inner = ColumnFutureCity;

        PredicatesLogic.Predicates predicate = new Not(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void Not_4_ColumnCount()
    {
        PredicatesLogic.Predicates inner = ParameterPi;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ColumnCount()
    {
        PredicatesLogic.Predicates inner = ParameterElite;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ColumnCount()
    {
        PredicatesLogic.Predicates inner = ParameterHelloWorld;

        PredicatesLogic.Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ColumnCount()
    {
        PredicatesLogic.Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ColumnName()
    {
        PredicatesLogic.Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        _ = and.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
        _ = and.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }
}
