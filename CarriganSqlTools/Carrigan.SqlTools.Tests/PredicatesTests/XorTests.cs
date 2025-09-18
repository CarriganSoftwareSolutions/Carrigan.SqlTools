using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

public class XorThanTests
{
    private static readonly PredicatesBase ColumnTastyPizza = new Columns<ColumnTable>("Pizza");
    private static readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private static readonly PredicatesBase ColumnDestructCode = new Columns<ColumnTable>("D000destruct0");
    private static readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private static readonly PredicatesBase ColumnFutureCity = new Columns<ColumnTable>("Express");
    private static readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private static readonly PredicatesBase ParameterPi = new Parameters("Pi", 3.14f);
    private static readonly string ParameterPiSql = "@Parameter_Pi";

    private static readonly PredicatesBase ParameterElite = new Parameters("Elite", 1337);
    private static readonly string ParameterEliteSql = "@Parameter_Elite";

    private static readonly PredicatesBase ParameterHelloWorld = new Parameters("HelloWorld", "Hello World!");
    private static readonly string ParameterHelloWorldSql = "@Parameter_HelloWorld";


    [Fact]
    public void Xor_1_ToSql()
    {
        PredicatesBase left = ColumnTastyPizza;
        string leftSql = ColumnTastyPizzaExpectedSql;

        PredicatesBase right = ColumnDestructCode;
        string rightSql = ColumnDestructCodeSql;

        PredicatesBase predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Xor_1_ParameterCount()
    {
        PredicatesBase left = ColumnTastyPizza;

        PredicatesBase right = ColumnDestructCode;

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 0;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ToSql()
    {
        PredicatesBase left = ColumnFutureCity;
        string leftSql = ColumnFutureCitySql;

        PredicatesBase right = ParameterPi;
        string rightSql = ParameterPiSql;

        PredicatesBase predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ParameterCount()
    {
        PredicatesBase left = ColumnFutureCity;

        PredicatesBase right = ParameterPi;

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 1;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ParameterValues()
    {
        PredicatesBase left = ColumnFutureCity;

        PredicatesBase right = ParameterPi;

        PredicatesBase predicate = new Xor(left, right);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.Parameter.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_3_ToSql()
    {
        PredicatesBase left = ParameterElite;
        string leftSql = ParameterEliteSql;

        PredicatesBase right = ParameterHelloWorld;
        string rightSql = ParameterHelloWorldSql;

        PredicatesBase predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_3_ParameterCount()
    {
        PredicatesBase left = ParameterElite;

        PredicatesBase right = ParameterHelloWorld;

        PredicatesBase predicate = new Xor(left, right);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.Parameter.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.Parameter.First(p => p.Name == "HelloWorld").Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Xor_Nested_ToSql()
    {
        PredicatesBase left = ParameterElite;
        string leftSql = ParameterEliteSql;

        PredicatesBase right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);
        string rightSql = $"({ParameterHelloWorldSql} AND {ColumnFutureCitySql} AND {ColumnDestructCodeSql})";

        PredicatesBase predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ParameterCount()
    {
        PredicatesBase left = ParameterElite;

        PredicatesBase right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ParameterValue()
    {
        PredicatesBase left = ParameterElite;

        PredicatesBase right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicatesBase predicate = new Xor(left, right);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.Parameter.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.Parameter.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Xor_1_ColumnCount()
    {
        PredicatesBase left = ColumnTastyPizza;

        PredicatesBase right = ColumnDestructCode;

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_1_ColumnName()
    {
        PredicatesBase left = ColumnTastyPizza;

        PredicatesBase right = ColumnDestructCode;

        PredicatesBase predicate = new Xor(left, right);


        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[Pizza]").Single();
        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void Xor_2_ColumnCount()
    {
        PredicatesBase left = ColumnFutureCity;

        PredicatesBase right = ParameterPi;

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 1;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ColumnName()
    {
        PredicatesBase left = ColumnFutureCity;

        PredicatesBase right = ParameterPi;

        PredicatesBase predicate = new Xor(left, right);

        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void Xor_3_ColumnCount()
    {
        PredicatesBase left = ParameterElite;

        PredicatesBase right = ParameterPi;

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 0;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ColumnCount()
    {
        PredicatesBase left = ParameterElite;

        PredicatesBase right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ColumnName()
    {
        PredicatesBase left = ParameterElite;

        PredicatesBase right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        PredicatesBase predicate = new Xor(left, right);

        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[Express]").Single();
    }
}
