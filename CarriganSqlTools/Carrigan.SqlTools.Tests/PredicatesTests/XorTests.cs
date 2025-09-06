using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.PredicatesTests;

public class XorThanTests
{
    private PredicatesBase ColumnPoppisTastyPizza = new Columns<ColumnTable>("Pizza");
    private string ColumnPoppisTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private PredicatesBase ColumnDestructCode = new Columns<ColumnTable>("D000descruct0");
    private string ColumnDestructCodeSql = "[ColumnTable].[D000descruct0]";

    private PredicatesBase ColumnFuturama = new Columns<ColumnTable>("Express");
    private string ColumnFuturamaSql = "[ColumnTable].[Express]";

    private PredicatesBase ParameterPi = new Parameters("Pi", 3.14f);
    private string ParameterPiSql = "@Parameter_Pi";

    private PredicatesBase ParamerterElite = new Parameters("Elite", 1337);
    private string ParamerterEliteSql = "@Parameter_Elite";

    private PredicatesBase ParamerterHelloWorld = new Parameters("HelloWorld", "Hello World!");
    private string ParamerterHelloWorldSql = "@Parameter_HelloWorld";


    [Fact]
    public void Xor_1_ToSql()
    {
        PredicatesBase left = ColumnPoppisTastyPizza;
        string leftSql = ColumnPoppisTastyPizzaExpectedSql;

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
        PredicatesBase left = ColumnPoppisTastyPizza;

        PredicatesBase right = ColumnDestructCode;

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 0;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ToSql()
    {
        PredicatesBase left = ColumnFuturama;
        string leftSql = ColumnFuturamaSql;

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
        PredicatesBase left = ColumnFuturama;

        PredicatesBase right = ParameterPi;

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 1;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ParameterValues()
    {
        PredicatesBase left = ColumnFuturama;

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
        PredicatesBase left = ParamerterElite;
        string leftSql = ParamerterEliteSql;

        PredicatesBase right = ParamerterHelloWorld;
        string rightSql = ParamerterHelloWorldSql;

        PredicatesBase predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_3_ParameterCount()
    {
        PredicatesBase left = ParamerterElite;

        PredicatesBase right = ParamerterHelloWorld;

        PredicatesBase predicate = new Xor(left, right);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.Parameter.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValuestring = (string)predicate.Parameter.Where(p => p.Name == "HelloWorld").First().Value;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValuestring);
    }

    [Fact]
    public void Xor_Nested_ToSql()
    {
        PredicatesBase left = ParamerterElite;
        string leftSql = ParamerterEliteSql;

        PredicatesBase right = new And(ParamerterHelloWorld, ColumnFuturama, ColumnDestructCode);
        string rightSql = $"({ParamerterHelloWorldSql} AND {ColumnFuturamaSql} AND {ColumnDestructCodeSql})";

        PredicatesBase predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ParameterCount()
    {
        PredicatesBase left = ParamerterElite;

        PredicatesBase right = new And(ParamerterHelloWorld, ColumnFuturama, ColumnDestructCode);

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Parameter.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ParameterValue()
    {
        PredicatesBase left = ParamerterElite;

        PredicatesBase right = new And(ParamerterHelloWorld, ColumnFuturama, ColumnDestructCode);

        PredicatesBase predicate = new Xor(left, right);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.Parameter.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValuestring = (string)predicate.Parameter.Where(p => p.Name == "HelloWorld").First().Value;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValuestring);
    }

    [Fact]
    public void Xor_1_ColumnCount()
    {
        PredicatesBase left = ColumnPoppisTastyPizza;

        PredicatesBase right = ColumnDestructCode;

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_1_ColumnName()
    {
        PredicatesBase left = ColumnPoppisTastyPizza;

        PredicatesBase right = ColumnDestructCode;

        PredicatesBase predicate = new Xor(left, right);


        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[Pizza]").Single();
        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[D000descruct0]").Single();
    }

    [Fact]
    public void Xor_2_ColumnCount()
    {
        PredicatesBase left = ColumnFuturama;

        PredicatesBase right = ParameterPi;

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 1;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ColumnName()
    {
        PredicatesBase left = ColumnFuturama;

        PredicatesBase right = ParameterPi;

        PredicatesBase predicate = new Xor(left, right);

        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void Xor_3_ColumnCount()
    {
        PredicatesBase left = ParamerterElite;

        PredicatesBase right = ParameterPi;

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 0;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ColumnCount()
    {
        PredicatesBase left = ParamerterElite;

        PredicatesBase right = new And(ParamerterHelloWorld, ColumnFuturama, ColumnDestructCode);

        PredicatesBase predicate = new Xor(left, right);

        int expectedValue = 2;
        int actualValue = predicate.Column.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ColumnName()
    {
        PredicatesBase left = ParamerterElite;

        PredicatesBase right = new And(ParamerterHelloWorld, ColumnFuturama, ColumnDestructCode);

        PredicatesBase predicate = new Xor(left, right);

        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[D000descruct0]").Single();
        _ = predicate.Column.Where(col => col.ColumnTag == "[ColumnTable].[Express]").Single();
    }
}
