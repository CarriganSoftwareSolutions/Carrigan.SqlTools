using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

public class IsNullTests
{
    private readonly PredicatesBase ColumnTastyPizza = new Columns<ColumnTable>("Pizza");
    private readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private readonly PredicatesBase ColumnDestructCode = new Columns<ColumnTable>("D000destruct0");
    private readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private readonly PredicatesBase ColumnFutureCity = new Columns<ColumnTable>("Express");
    private readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private readonly PredicatesBase ParameterPi = new Parameters("Pi", 3.14f);
    private readonly string ParameterPiSql = "@Parameter_Pi";

    private readonly PredicatesBase ParameterElite = new Parameters("Elite", 1337);
    private readonly string ParameterEliteSql = "@Parameter_Elite";

    private readonly PredicatesBase ParameterHelloWorld = new Parameters("HelloWorld", "Hello World!");
    private readonly string ParameterHelloWorldSql = "@Parameter_HelloWorld";


    [Fact]
    public void IsNull_1_ToSql()
    {
        PredicatesBase inner = ColumnTastyPizza;
        string innerSql = ColumnTastyPizzaExpectedSql;

        PredicatesBase predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void IsNull_1_ParameterCount()
    {
        PredicatesBase inner = ColumnTastyPizza;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_2_ToSql()
    {
        PredicatesBase inner = ColumnDestructCode;
        string innerSql = ColumnDestructCodeSql;

        PredicatesBase predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void IsNull_2_ParameterCount()
    {
        PredicatesBase inner = ColumnDestructCode;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_3_ToSql()
    {
        PredicatesBase inner = ColumnFutureCity;
        string innerSql = ColumnFutureCitySql;

        PredicatesBase predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_3_ParameterCount()
    {
        PredicatesBase inner = ColumnFutureCity;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_4_ToSql()
    {
        PredicatesBase inner = ParameterPi;
        string innerSql = ParameterPiSql;

        PredicatesBase predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_4_ParameterCount()
    {
        PredicatesBase inner = ParameterPi;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_4_ParameterValues()
    {
        PredicatesBase inner = ParameterPi;

        PredicatesBase predicate = new IsNull(inner);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.Parameter.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }


    [Fact]
    public void IsNull_5_ToSql()
    {
        PredicatesBase inner = ParameterElite;
        string innerSql = ParameterEliteSql;

        PredicatesBase predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_5_ParameterCount()
    {
        PredicatesBase inner = ParameterElite;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_5_ParameterValue()
    {
        PredicatesBase inner = ParameterElite;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.Parameter.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        Assert.Equal(expectedValueInt, actualValueInt);
    }






    [Fact]
    public void IsNull_6_ToSql()
    {
        PredicatesBase inner = ParameterHelloWorld;
        string innerSql = ParameterHelloWorldSql;

        PredicatesBase predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_6_ParameterCount()
    {
        PredicatesBase inner = ParameterHelloWorld;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_6_ParameterValue()
    {

        PredicatesBase inner = ParameterHelloWorld;

        PredicatesBase predicate = new IsNull(inner);

        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.Parameter.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void IsNull_Nested_ToSql()
    {
        PredicatesBase and = new And(new IsNull(ParameterElite), new IsNull(ParameterHelloWorld), new IsNull(ColumnFutureCity), new IsNull(ColumnDestructCode));
        string andSql = $"(({ParameterEliteSql} IS NULL) AND ({ParameterHelloWorldSql} IS NULL) AND ({ColumnFutureCitySql} IS NULL) AND ({ColumnDestructCodeSql} IS NULL))";

        string expectedValue = andSql;
        string actualValue = and.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_Nested_ParameterCount()
    {
        PredicatesBase and = new And(new IsNull(ParameterElite), new IsNull(ParameterHelloWorld), new IsNull(ColumnFutureCity), new IsNull(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_Nested_ParameterValue()
    {
        PredicatesBase and = new And(new IsNull(ParameterElite), new IsNull(ParameterHelloWorld), new IsNull(ColumnFutureCity), new IsNull(ColumnDestructCode));

        int expectedValueInt = 1337;
        object? nullableActualValueInt = and.Parameter.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)and.Parameter.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void IsNull_1_ColumnCount()
    {
        PredicatesBase inner = ColumnTastyPizza;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_1_ColumnName()
    {
        PredicatesBase inner = ColumnTastyPizza;

        PredicatesBase predicate = new IsNull(inner);

        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[Pizza]").Single();
    }

    [Fact]
    public void IsNull_2_ColumnCount()
    {
        PredicatesBase inner = ColumnDestructCode;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_2_ColumnName()
    {
        PredicatesBase inner = ColumnDestructCode;

        PredicatesBase predicate = new IsNull(inner);

        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void IsNull_3_ColumnCount()
    {
        PredicatesBase inner = ColumnFutureCity;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_3_ColumnName()
    {
        PredicatesBase inner = ColumnFutureCity;

        PredicatesBase predicate = new IsNull(inner);

        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void IsNull_4_ColumnCount()
    {
        PredicatesBase inner = ParameterPi;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_5_ColumnCount()
    {
        PredicatesBase inner = ParameterElite;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_6_ColumnCount()
    {
        PredicatesBase inner = ParameterHelloWorld;

        PredicatesBase predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_Nested_ColumnCount()
    {
        PredicatesBase and = new And(new IsNull(ParameterElite), new IsNull(ParameterHelloWorld), new IsNull(ColumnFutureCity), new IsNull(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_Nested_ColumnName()
    {
        PredicatesBase and = new And(new IsNull(ParameterElite), new IsNull(ParameterHelloWorld), new IsNull(ColumnFutureCity), new IsNull(ColumnDestructCode));

        _ = and.Column.Where(col => col.ColumnTag == "[ColumnTable].[D000destruct0]").Single();
        _ = and.Column.Where(col => col.ColumnTag == "[ColumnTable].[Express]").Single();
    }
}
