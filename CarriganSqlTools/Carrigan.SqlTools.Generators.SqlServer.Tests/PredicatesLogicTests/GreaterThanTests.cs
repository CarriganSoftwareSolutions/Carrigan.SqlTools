using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class GreaterThanTests
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
    private readonly string ParameterHelloWorldSql = "@HelloWorld_2";


    [Fact]
    public void GreaterThan_1_ToSql()
    {
        Predicates left = ColumnTastyPizza;
        string leftSql = ColumnTastyPizzaExpectedSql;

        Predicates right = ColumnDestructCode;
        string rightSql = ColumnDestructCodeSql;

        Predicates predicate = new GreaterThan(left, right);

        string expectedValue = $"({leftSql} > {rightSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void GreaterThan_1_ParameterCount()
    {
        Predicates left = ColumnTastyPizza;

        Predicates right = ColumnDestructCode;

        Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_2_ToSql()
    {
        Predicates left = ColumnFutureCity;
        string leftSql = ColumnFutureCitySql;

        Predicates right = ParameterPi;
        string rightSql = ParameterPiSql;

        Predicates predicate = new GreaterThan(left, right);

        string expectedValue = $"({leftSql} > {rightSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_2_ParameterCount()
    {
        Predicates left = ColumnFutureCity;

        Predicates right = ParameterPi;

        Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_2_ParameterValues()
    {
        Predicates left = ColumnFutureCity;

        Predicates right = ParameterPi;

        Predicates predicate = new GreaterThan(left, right);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.DescendantParameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_3_ToSql()
    {
        Predicates left = ParameterElite;
        string leftSql = ParameterEliteSql;

        Predicates right = ParameterHelloWorld;
        string rightSql = ParameterHelloWorldSql;

        Predicates predicate = new GreaterThan(left, right);

        string expectedValue = $"({leftSql} > {rightSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_3_ParameterCount()
    {
        Predicates left = ParameterElite;

        Predicates right = ParameterHelloWorld;

        Predicates predicate = new GreaterThan(left, right);

        int expectedValueInt = 1337; object? nullableActualValueInt = predicate.DescendantParameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = ((string?)predicate.DescendantParameters.Where(p => p.Name == "HelloWorld").First().Value) ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void GreaterThan_Nested_ToSql()
    {
        Predicates left = ParameterElite;
        string leftSql = ParameterEliteSql;

        Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);
        string rightSql = $"({ParameterHelloWorldSql} AND {ColumnFutureCitySql} AND {ColumnDestructCodeSql})";

        Predicates predicate = new GreaterThan(left, right);

        string expectedValue = $"({leftSql} > {rightSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_Nested_ParameterCount()
    {
        Predicates left = ParameterElite;

        Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_Nested_ParameterValue()
    {
        Predicates left = ParameterElite;

        Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        Predicates predicate = new GreaterThan(left, right);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.DescendantParameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.DescendantParameters.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void GreaterThan_1_ColumnCount()
    {
        Predicates left = ColumnTastyPizza;

        Predicates right = ColumnDestructCode;

        Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_1_ColumnName()
    {
        Predicates left = ColumnTastyPizza;

        Predicates right = ColumnDestructCode;

        Predicates predicate = new GreaterThan(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Pizza").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.D000destruct0").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Pizza]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void GreaterThan_2_ColumnCount()
    {
        Predicates left = ColumnFutureCity;

        Predicates right = ParameterPi;

        Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_2_ColumnName()
    {
        Predicates left = ColumnFutureCity;

        Predicates right = ParameterPi;

        Predicates predicate = new GreaterThan(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Express]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Express").Single();
    }

    [Fact]
    public void GreaterThan_3_ColumnCount()
    {
        Predicates left = ParameterElite;

        Predicates right = ParameterPi;

        Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_Nested_ColumnCount()
    {
        Predicates left = ParameterElite;

        Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        Predicates predicate = new GreaterThan(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GreaterThan_Nested_ColumnName()
    {
        Predicates left = ParameterElite;

        Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        Predicates predicate = new GreaterThan(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Express]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.D000destruct0").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Express").Single();
    }

    [Fact]
    public void GreaterThan_LeftNull_Throws() =>
    Assert.Throws<NullReferenceException>(() => new GreaterThan(null!, new Parameter(1, "P1")));

    [Fact]
    public void GreaterThan_RightNull_Throws() =>
        Assert.Throws<NullReferenceException>(() => new GreaterThan(new Parameter(1, "P1"), null!));
}