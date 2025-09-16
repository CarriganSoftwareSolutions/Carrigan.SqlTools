using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

public class NotTests
{
    private readonly PredicatesBase ColumnPoppisTastyPizza = new Columns<ColumnTable>("Pizza");
    private readonly string ColumnPoppisTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private readonly PredicatesBase ColumnDestructCode = new Columns<ColumnTable>("D000descruct0");
    private readonly string ColumnDestructCodeSql = "[ColumnTable].[D000descruct0]";

    private readonly PredicatesBase ColumnFuturama = new Columns<ColumnTable>("Express");
    private readonly string ColumnFuturamaSql = "[ColumnTable].[Express]";

    private readonly PredicatesBase ParameterPi = new Parameters("Pi", 3.14f);
    private readonly string ParameterPiSql = "@Parameter_Pi";

    private readonly PredicatesBase ParamerterElite = new Parameters("Elite", 1337);
    private readonly string ParamerterEliteSql = "@Parameter_Elite";

    private readonly PredicatesBase ParamerterHelloWorld = new Parameters("HelloWorld", "Hello World!");
    private readonly string ParamerterHelloWorldSql = "@Parameter_HelloWorld";


    [Fact]
    public void Not_1_ToSql()
    {
        PredicatesBase inner = ColumnPoppisTastyPizza;
        string innerSql = ColumnPoppisTastyPizzaExpectedSql;

        PredicatesBase predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Not_1_ParameterCount()
    {
        PredicatesBase inner = ColumnPoppisTastyPizza;

        PredicatesBase predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_2_ToSql()
    {
        PredicatesBase inner = ColumnDestructCode;
        string innerSql = ColumnDestructCodeSql;

        PredicatesBase predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Not_2_ParameterCount()
    {
        PredicatesBase inner = ColumnDestructCode;

        PredicatesBase predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ToSql()
    {
        PredicatesBase inner = ColumnFuturama;
        string innerSql = ColumnFuturamaSql;

        PredicatesBase predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ParameterCount()
    {
        PredicatesBase inner = ColumnFuturama;

        PredicatesBase predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ToSql()
    {
        PredicatesBase inner = ParameterPi;
        string innerSql = ParameterPiSql;

        PredicatesBase predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ParameterCount()
    {
        PredicatesBase inner = ParameterPi;

        PredicatesBase predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_4_ParameterValues()
    {
        PredicatesBase inner = ParameterPi;

        PredicatesBase predicate = new Not(inner);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.Parameter.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }


    [Fact]
    public void Not_5_ToSql()
    {
        PredicatesBase inner = ParamerterElite;
        string innerSql = ParamerterEliteSql;

        PredicatesBase predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ParameterCount()
    {
        PredicatesBase inner = ParamerterElite;

        PredicatesBase predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ParameterValue()
    {
        PredicatesBase inner = ParamerterElite;

        PredicatesBase predicate = new Not(inner);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.Parameter.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        Assert.Equal(expectedValueInt, actualValueInt);
    }






    [Fact]
    public void Not_6_ToSql()
    {
        PredicatesBase inner = ParamerterHelloWorld;
        string innerSql = ParamerterHelloWorldSql;

        PredicatesBase predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ParameterCount()
    {
        PredicatesBase inner = ParamerterHelloWorld;

        PredicatesBase predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ParameterValue()
    {

        PredicatesBase inner = ParamerterHelloWorld;

        PredicatesBase predicate = new Not(inner);

        string expectedValueString = "Hello World!";
        string actualValuestring = (string?)predicate.Parameter.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueString, actualValuestring);
    }

    [Fact]
    public void Not_Nested_ToSql()
    {
        PredicatesBase and = new And(new Not(ParamerterElite), new Not(ParamerterHelloWorld), new Not(ColumnFuturama), new Not(ColumnDestructCode));
        string andSql = $"((NOT {ParamerterEliteSql}) AND (NOT {ParamerterHelloWorldSql}) AND (NOT {ColumnFuturamaSql}) AND (NOT {ColumnDestructCodeSql}))";

        string expectedValue = andSql;
        string actualValue = and.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ParameterCount()
    {
        PredicatesBase and = new And(new Not(ParamerterElite), new Not(ParamerterHelloWorld), new Not(ColumnFuturama), new Not(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ParameterValue()
    {
        PredicatesBase and = new And(new Not(ParamerterElite), new Not(ParamerterHelloWorld), new Not(ColumnFuturama), new Not(ColumnDestructCode));

        int expectedValueInt = 1337;
        object? nullableActualValueInt = and.Parameter.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValuestring = (string?)and.Parameter.First(p => p.Name == "HelloWorld").Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValuestring);
    }

    [Fact]
    public void Not_1_ColumnCount()
    {
        PredicatesBase inner = ColumnPoppisTastyPizza;

        PredicatesBase predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_1_ColumnName()
    {
        PredicatesBase inner = ColumnPoppisTastyPizza;

        PredicatesBase predicate = new Not(inner);

        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[Pizza]").Single();
    }

    [Fact]
    public void Not_2_ColumnCount()
    {
        PredicatesBase inner = ColumnDestructCode;

        PredicatesBase predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_2_ColumnName()
    {
        PredicatesBase inner = ColumnDestructCode;

        PredicatesBase predicate = new Not(inner);

        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[D000descruct0]").Single();
    }

    [Fact]
    public void Not_3_ColumnCount()
    {
        PredicatesBase inner = ColumnFuturama;

        PredicatesBase predicate = new Not(inner);

        int expectedValue = 1;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_3_ColumnName()
    {
        PredicatesBase inner = ColumnFuturama;

        PredicatesBase predicate = new Not(inner);

        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void Not_4_ColumnCount()
    {
        PredicatesBase inner = ParameterPi;

        PredicatesBase predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_5_ColumnCount()
    {
        PredicatesBase inner = ParamerterElite;

        PredicatesBase predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_6_ColumnCount()
    {
        PredicatesBase inner = ParamerterHelloWorld;

        PredicatesBase predicate = new Not(inner);

        int expectedValue = 0;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ColumnCount()
    {
        PredicatesBase and = new And(new Not(ParamerterElite), new Not(ParamerterHelloWorld), new Not(ColumnFuturama), new Not(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ColumnName()
    {
        PredicatesBase and = new And(new Not(ParamerterElite), new Not(ParamerterHelloWorld), new Not(ColumnFuturama), new Not(ColumnDestructCode));

        _ = and.Column.Where(col => col.ColumnTag == "[ColumnTable].[D000descruct0]").Single();
        _ = and.Column.Where(col => col.ColumnTag == "[ColumnTable].[Express]").Single();
    }
}
