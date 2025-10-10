using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

public class IsNotNullTests
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
    public void IsNotNull_1_ToSql()
    {
        Predicates.Predicates inner = ColumnTastyPizza;
        string innerSql = ColumnTastyPizzaExpectedSql;

        Predicates.Predicates predicate = new IsNotNull(inner);

        string expectedValue = $"({innerSql} IS NOT NULL)";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void IsNotNull_1_ParameterCount()
    {
        Predicates.Predicates inner = ColumnTastyPizza;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_2_ToSql()
    {
        Predicates.Predicates inner = ColumnDestructCode;
        string innerSql = ColumnDestructCodeSql;

        Predicates.Predicates predicate = new IsNotNull(inner);

        string expectedValue = $"({innerSql} IS NOT NULL)";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void IsNotNull_2_ParameterCount()
    {
        Predicates.Predicates inner = ColumnDestructCode;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_3_ToSql()
    {
        Predicates.Predicates inner = ColumnFutureCity;
        string innerSql = ColumnFutureCitySql;

        Predicates.Predicates predicate = new IsNotNull(inner);

        string expectedValue = $"({innerSql} IS NOT NULL)";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_3_ParameterCount()
    {
        Predicates.Predicates inner = ColumnFutureCity;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_4_ToSql()
    {
        Predicates.Predicates inner = ParameterPi;
        string innerSql = ParameterPiSql;

        Predicates.Predicates predicate = new IsNotNull(inner);

        string expectedValue = $"({innerSql} IS NOT NULL)";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_4_ParameterCount()
    {
        Predicates.Predicates inner = ParameterPi;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_4_ParameterValues()
    {
        Predicates.Predicates inner = ParameterPi;

        Predicates.Predicates predicate = new IsNotNull(inner);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.Parameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }


    [Fact]
    public void IsNotNull_5_ToSql()
    {
        Predicates.Predicates inner = ParameterElite;
        string innerSql = ParameterEliteSql;

        Predicates.Predicates predicate = new IsNotNull(inner);

        string expectedValue = $"({innerSql} IS NOT NULL)";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_5_ParameterCount()
    {
        Predicates.Predicates inner = ParameterElite;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_5_ParameterValue()
    {
        Predicates.Predicates inner = ParameterElite;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.Parameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        Assert.Equal(expectedValueInt, actualValueInt);
    }


    [Fact]
    public void IsNotNull_6_ToSql()
    {
        Predicates.Predicates inner = ParameterHelloWorld;
        string innerSql = ParameterHelloWorldSql;

        Predicates.Predicates predicate = new IsNotNull(inner);

        string expectedValue = $"({innerSql} IS NOT NULL)";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_6_ParameterCount()
    {
        Predicates.Predicates inner = ParameterHelloWorld;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_6_ParameterValue()
    {

        Predicates.Predicates inner = ParameterHelloWorld;

        Predicates.Predicates predicate = new IsNotNull(inner);

        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.Parameters.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void IsNotNull_Nested_ToSql()
    {
        Predicates.Predicates and = new And(new IsNotNull(ParameterElite), new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));
        string andSql = $"(({ParameterEliteSql} IS NOT NULL) AND ({ParameterHelloWorldSql} IS NOT NULL) AND ({ColumnFutureCitySql} IS NOT NULL) AND ({ColumnDestructCodeSql} IS NOT NULL))";

        string expectedValue = andSql;
        string actualValue = and.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_Nested_ParameterCount()
    {
        Predicates.Predicates and = new And(new IsNotNull(ParameterElite), new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_Nested_ParameterValue()
    {
        Predicates.Predicates and = new And(new IsNotNull(ParameterElite), new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        int expectedValueInt = 1337;
        object? nullableActualValueInt = and.Parameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)and.Parameters.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }


    [Fact]
    public void IsNotNull_1_ColumnCount()
    {
        Predicates.Predicates inner = ColumnTastyPizza;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_1_ColumnName()
    {
        Predicates.Predicates inner = ColumnTastyPizza;

        Predicates.Predicates predicate = new IsNotNull(inner);


        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Pizza]").Single();
    }

    [Fact]
    public void IsNotNull_2_ColumnCount()
    {
        Predicates.Predicates inner = ColumnDestructCode;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_2_ColumnName()
    {
        Predicates.Predicates inner = ColumnDestructCode;

        Predicates.Predicates predicate = new IsNotNull(inner);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void IsNotNull_3_ColumnCount()
    {
        Predicates.Predicates inner = ColumnFutureCity;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_3_ColumnName()
    {
        Predicates.Predicates inner = ColumnFutureCity;

        Predicates.Predicates predicate = new IsNotNull(inner);

        _ = predicate.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void IsNotNull_4_ColumnCount()
    {
        Predicates.Predicates inner = ParameterPi;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_5_ColumnCount()
    {
        Predicates.Predicates inner = ParameterElite;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_6_ColumnCount()
    {
        Predicates.Predicates inner = ParameterHelloWorld;

        Predicates.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.Columns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_Nested_ColumnCount()
    {
        Predicates.Predicates and = new And(new IsNotNull(ParameterElite), new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.Parameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_Nested_ColumnName()
    {
        Predicates.Predicates and = new And(new IsNotNull(ParameterElite), new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        _ = and.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
        _ = and.Columns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }
}
