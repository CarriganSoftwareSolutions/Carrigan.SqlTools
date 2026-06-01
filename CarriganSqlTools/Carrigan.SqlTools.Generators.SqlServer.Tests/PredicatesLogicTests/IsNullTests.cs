using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class IsNullTests
{
    private static readonly SqlServerDialect Dialect = new();

    private readonly Predicates ColumnTastyPizza = new Column<ColumnTable>("Pizza");
    private readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private readonly Predicates ColumnDestructCode = new Column<ColumnTable>("D000destruct0");
    private readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private readonly Predicates ColumnFutureCity = new Column<ColumnTable>("Express");
    private readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private readonly Predicates ParameterPi = new Parameter(3.14f, "Pi");
    private readonly string ParameterPiSql = "@Pi_1";

    private readonly Predicates ParameterElite = new Parameter(1337, "Elite");
    private readonly string ParameterEliteSql = "@Elite_1";

    private readonly Predicates ParameterHelloWorld = new Parameter("Hello World!", "HelloWorld");
    private readonly string ParameterHelloWorldSql = "@HelloWorld_1";


    [Fact]
    public void IsNull_1_ToSql()
    {
        Predicates inner = ColumnTastyPizza;
        string innerSql = ColumnTastyPizzaExpectedSql;

        Predicates predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void IsNull_1_ParameterCount()
    {
        Predicates inner = ColumnTastyPizza;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_2_ToSql()
    {
        Predicates inner = ColumnDestructCode;
        string innerSql = ColumnDestructCodeSql;

        Predicates predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void IsNull_2_ParameterCount()
    {
        Predicates inner = ColumnDestructCode;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_3_ToSql()
    {
        Predicates inner = ColumnFutureCity;
        string innerSql = ColumnFutureCitySql;

        Predicates predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_3_ParameterCount()
    {
        Predicates inner = ColumnFutureCity;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_4_ToSql()
    {
        Predicates inner = ParameterPi;
        string innerSql = ParameterPiSql;

        Predicates predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_4_ParameterCount()
    {
        Predicates inner = ParameterPi;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_4_ParameterValues()
    {
        Predicates inner = ParameterPi;

        Predicates predicate = new IsNull(inner);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.DescendantParameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }


    [Fact]
    public void IsNull_5_ToSql()
    {
        Predicates inner = ParameterElite;
        string innerSql = ParameterEliteSql;

        Predicates predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_5_ParameterCount()
    {
        Predicates inner = ParameterElite;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_5_ParameterValue()
    {
        Predicates inner = ParameterElite;

        Predicates predicate = new IsNull(inner);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.DescendantParameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        Assert.Equal(expectedValueInt, actualValueInt);
    }






    [Fact]
    public void IsNull_6_ToSql()
    {
        Predicates inner = ParameterHelloWorld;
        string innerSql = ParameterHelloWorldSql;

        Predicates predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_6_ParameterCount()
    {
        Predicates inner = ParameterHelloWorld;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_6_ParameterValue()
    {

        Predicates inner = ParameterHelloWorld;

        Predicates predicate = new IsNull(inner);

        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.DescendantParameters.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void IsNull_Nested_ToSql()
    {
        Predicates and = new And(new IsNull(ParameterElite), new IsNull(ParameterHelloWorld), new IsNull(ColumnFutureCity), new IsNull(ColumnDestructCode));
        string andSql = $"((@Elite_1 IS NULL) AND (@HelloWorld_2 IS NULL) AND ({ColumnFutureCitySql} IS NULL) AND ({ColumnDestructCodeSql} IS NULL))";

        string expectedValue = andSql;
        string actualValue = and.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_Nested_ParameterCount()
    {
        Predicates and = new And(new IsNull(ParameterElite), new IsNull(ParameterHelloWorld), new IsNull(ColumnFutureCity), new IsNull(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_Nested_ParameterValue()
    {
        Predicates and = new And(new IsNull(ParameterElite), new IsNull(ParameterHelloWorld), new IsNull(ColumnFutureCity), new IsNull(ColumnDestructCode));

        int expectedValueInt = 1337;
        object? nullableActualValueInt = and.DescendantParameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)and.DescendantParameters.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void IsNull_1_ColumnCount()
    {
        Predicates inner = ColumnTastyPizza;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_1_ColumnName()
    {
        Predicates inner = ColumnTastyPizza;

        Predicates predicate = new IsNull(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Pizza]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Pizza").Single();
    }

    [Fact]
    public void IsNull_2_ColumnCount()
    {
        Predicates inner = ColumnDestructCode;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_2_ColumnName()
    {
        Predicates inner = ColumnDestructCode;

        Predicates predicate = new IsNull(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.D000destruct0").Single();
    }

    [Fact]
    public void IsNull_3_ColumnCount()
    {
        Predicates inner = ColumnFutureCity;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_3_ColumnName()
    {
        Predicates inner = ColumnFutureCity;

        Predicates predicate = new IsNull(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Express]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Express").Single();
    }

    [Fact]
    public void IsNull_4_ColumnCount()
    {
        Predicates inner = ParameterPi;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_5_ColumnCount()
    {
        Predicates inner = ParameterElite;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_6_ColumnCount()
    {
        Predicates inner = ParameterHelloWorld;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_Nested_ColumnCount()
    {
        Predicates and = new And(new IsNull(ParameterElite), new IsNull(ParameterHelloWorld), new IsNull(ColumnFutureCity), new IsNull(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_Nested_ColumnName()
    {
        Predicates and = new And(new IsNull(ParameterElite), new IsNull(ParameterHelloWorld), new IsNull(ColumnFutureCity), new IsNull(ColumnDestructCode));

        _ = and.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[D000destruct0]").Single();
        _ = and.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Express]").Single();
        _ = and.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.D000destruct0").Single();
        _ = and.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Express").Single();
    }

    [Fact]
    public void IsNull_NullValue_ThrowsArgumentNullException() =>
    Assert.Throws<ArgumentNullException>(() => new IsNull(null!));

}