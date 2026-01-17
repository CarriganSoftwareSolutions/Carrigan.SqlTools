using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

public class NotTests
{
    private readonly Predicates ColumnTastyPizza = new Column<ColumnTable>("Pizza");
    private readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private readonly Predicates ColumnDestructCode = new Column<ColumnTable>("D000destruct0");
    private readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private readonly Predicates ColumnFutureCity = new Column<ColumnTable>("Express");
    private readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private readonly Predicates ParameterPi = new Parameter("Pi", 3.14f, null);
    private readonly string ParameterPiSql = "@Parameter_Pi";

    private readonly Predicates ParameterElite = new Parameter("Elite", 1337, null);
    private readonly string ParameterEliteSql = "@Parameter_Elite";

    private readonly Predicates ParameterHelloWorld = new Parameter("HelloWorld", "Hello World!", null);
    private readonly string ParameterHelloWorldSql = "@Parameter_HelloWorld";


    [Fact]
    public void Not_1_ToSql()
    {
        Predicates inner = ColumnTastyPizza;
        string innerSql = ColumnTastyPizzaExpectedSql;

        Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Not_1_ParameterCount()
    {
        Predicates inner = ColumnTastyPizza;

        Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_2_ToSql()
    {
        Predicates inner = ColumnDestructCode;
        string innerSql = ColumnDestructCodeSql;

        Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Not_2_ParameterCount()
    {
        Predicates inner = ColumnDestructCode;

        Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ToSql()
    {
        Predicates inner = ColumnFutureCity;
        string innerSql = ColumnFutureCitySql;

        Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ParameterCount()
    {
        Predicates inner = ColumnFutureCity;

        Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ToSql()
    {
        Predicates inner = ParameterPi;
        string innerSql = ParameterPiSql;

        Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ParameterCount()
    {
        Predicates inner = ParameterPi;

        Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ParameterValues()
    {
        Predicates inner = ParameterPi;

        Predicates predicate = new Not(inner);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.DescendantParameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }


    [Fact]
    public void Not_5_ToSql()
    {
        Predicates inner = ParameterElite;
        string innerSql = ParameterEliteSql;

        Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ParameterCount()
    {
        Predicates inner = ParameterElite;

        Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ParameterValue()
    {
        Predicates inner = ParameterElite;

        Predicates predicate = new Not(inner);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.DescendantParameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        Assert.Equal(expectedValueInt, actualValueInt);
    }






    [Fact]
    public void Not_6_ToSql()
    {
        Predicates inner = ParameterHelloWorld;
        string innerSql = ParameterHelloWorldSql;

        Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ParameterCount()
    {
        Predicates inner = ParameterHelloWorld;

        Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ParameterValue()
    {

        Predicates inner = ParameterHelloWorld;

        Predicates predicate = new Not(inner);

        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.DescendantParameters.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Not_Nested_ToSql()
    {
        Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));
        string andSql = $"((NOT {ParameterEliteSql}) AND (NOT {ParameterHelloWorldSql}) AND (NOT {ColumnFutureCitySql}) AND (NOT {ColumnDestructCodeSql}))";

        string expectedValue = andSql;
        string actualValue = and.ToSqlFragments("Parameter").ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ParameterCount()
    {
        Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ParameterValue()
    {
        Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

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
        Predicates inner = ColumnTastyPizza;

        Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_1_ColumnName()
    {
        Predicates inner = ColumnTastyPizza;

        Predicates predicate = new Not(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[Pizza]").Single();
    }

    [Fact]
    public void Not_2_ColumnCount()
    {
        Predicates inner = ColumnDestructCode;

        Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_2_ColumnName()
    {
        Predicates inner = ColumnDestructCode;

        Predicates predicate = new Not(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void Not_3_ColumnCount()
    {
        Predicates inner = ColumnFutureCity;

        Predicates predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ColumnName()
    {
        Predicates inner = ColumnFutureCity;

        Predicates predicate = new Not(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void Not_4_ColumnCount()
    {
        Predicates inner = ParameterPi;

        Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ColumnCount()
    {
        Predicates inner = ParameterElite;

        Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ColumnCount()
    {
        Predicates inner = ParameterHelloWorld;

        Predicates predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ColumnCount()
    {
        Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ColumnName()
    {
        Predicates and = new And(new Not(ParameterElite), new Not(ParameterHelloWorld), new Not(ColumnFutureCity), new Not(ColumnDestructCode));

        _ = and.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
        _ = and.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void Not_NullPredicate_ThrowsArgumentNullException() =>
    Assert.Throws<ArgumentNullException>(() => new Not(null!));

}
