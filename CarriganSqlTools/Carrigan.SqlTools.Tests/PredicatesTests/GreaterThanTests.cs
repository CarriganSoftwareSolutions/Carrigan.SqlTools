using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

public class GreaterThanTests
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
    public void GreaterThan_1_ToSql()
    {
        Predicates.Predicates left = ColumnTastyPizza;
        string leftSql = ColumnTastyPizzaExpectedSql;

        Predicates.Predicates right = ColumnDestructCode;
        string rightSql = ColumnDestructCodeSql;

        Predicates.Predicates predicate = new GreaterThan(left, right);

        string expectedValue = $"({leftSql} > {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void GreaterThan_1_ParameterCount()
    {
        Predicates.Predicates left = ColumnTastyPizza;

        Predicates.Predicates right = ColumnDestructCode;

        Predicates.Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 0;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_2_ToSql()
    {
        Predicates.Predicates left = ColumnFutureCity;
        string leftSql = ColumnFutureCitySql;

        Predicates.Predicates right = ParameterPi;
        string rightSql = ParameterPiSql;

        Predicates.Predicates predicate = new GreaterThan(left, right);

        string expectedValue = $"({leftSql} > {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_2_ParameterCount()
    {
        Predicates.Predicates left = ColumnFutureCity;

        Predicates.Predicates right = ParameterPi;

        Predicates.Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 1;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_2_ParameterValues()
    {
        Predicates.Predicates left = ColumnFutureCity;

        Predicates.Predicates right = ParameterPi;

        Predicates.Predicates predicate = new GreaterThan(left, right);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.Parameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_3_ToSql()
    {
        Predicates.Predicates left = ParameterElite;
        string leftSql = ParameterEliteSql;

        Predicates.Predicates right = ParameterHelloWorld;
        string rightSql = ParameterHelloWorldSql;

        Predicates.Predicates predicate = new GreaterThan(left, right);

        string expectedValue = $"({leftSql} > {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_3_ParameterCount()
    {
        Predicates.Predicates left = ParameterElite;

        Predicates.Predicates right = ParameterHelloWorld;

        Predicates.Predicates predicate = new GreaterThan(left, right);

        int expectedValueInt = 1337; object? nullableActualValueInt = predicate.Parameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = ((string?)predicate.Parameters.Where(p => p.Name == "HelloWorld").First().Value) ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void GreaterThan_Nested_ToSql()
    {
        Predicates.Predicates left = ParameterElite;
        string leftSql = ParameterEliteSql;

        Predicates.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);
        string rightSql = $"({ParameterHelloWorldSql} AND {ColumnFutureCitySql} AND {ColumnDestructCodeSql})";

        Predicates.Predicates predicate = new GreaterThan(left, right);

        string expectedValue = $"({leftSql} > {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_Nested_ParameterCount()
    {
        Predicates.Predicates left = ParameterElite;

        Predicates.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        Predicates.Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_Nested_ParameterValue()
    {
        Predicates.Predicates left = ParameterElite;

        Predicates.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        Predicates.Predicates predicate = new GreaterThan(left, right);

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
    public void GreaterThan_1_ColumnCount()
    {
        Predicates.Predicates left = ColumnTastyPizza;

        Predicates.Predicates right = ColumnDestructCode;

        Predicates.Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_1_ColumnName()
    {
        Predicates.Predicates left = ColumnTastyPizza;

        Predicates.Predicates right = ColumnDestructCode;

        Predicates.Predicates predicate = new GreaterThan(left, right);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Pizza]").Single();
        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void GreaterThan_2_ColumnCount()
    {
        Predicates.Predicates left = ColumnFutureCity;

        Predicates.Predicates right = ParameterPi;

        Predicates.Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 1;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_2_ColumnName()
    {
        Predicates.Predicates left = ColumnFutureCity;

        Predicates.Predicates right = ParameterPi;

        Predicates.Predicates predicate = new GreaterThan(left, right);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void GreaterThan_3_ColumnCount()
    {
        Predicates.Predicates left = ParameterElite;

        Predicates.Predicates right = ParameterPi;

        Predicates.Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 0;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_Nested_ColumnCount()
    {
        Predicates.Predicates left = ParameterElite;

        Predicates.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        Predicates.Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_Nested_ColumnName()
    {
        Predicates.Predicates left = ParameterElite;

        Predicates.Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        Predicates.Predicates predicate = new GreaterThan(left, right);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }
}
