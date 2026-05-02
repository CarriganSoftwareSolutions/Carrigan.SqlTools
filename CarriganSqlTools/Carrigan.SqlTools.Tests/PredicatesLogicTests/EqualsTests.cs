using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

public class EqualsTests
{
    private readonly Predicates ColumnTastyPizza = new Column<ColumnTable>("Pizza");
    private readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private readonly Predicates ColumnDestructCode = new Column<ColumnTable>("D000destruct0");
    private readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private readonly Predicates ColumnFutureCity = new Column<ColumnTable>("Express");
    private readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private readonly Predicates ParameterPi = new Parameter("Pi", 3.14f, null);

    private readonly Predicates ParameterElite = new Parameter("Elite", 1337, SqlTypeDefinition.AsInt());

    private readonly Predicates ParameterHelloWorld = new Parameter("HelloWorld", "Hello World!");


    [Fact]
    public void Equals_ToSql1()
    {
        Predicates left = ColumnTastyPizza;
        string leftSql = ColumnTastyPizzaExpectedSql;

        Predicates right = ColumnDestructCode;
        string rightSql = ColumnDestructCodeSql;

        Predicates predicate = new Equal(left, right);

        string expectedValue = $"({leftSql} = {rightSql})";
        string actualValue = predicate.ToSqlFragments().ToSql(new SqlServerDialect());

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Equals1_ParameterCount()
    {
        Predicates left = ColumnTastyPizza;

        Predicates right = ColumnDestructCode;

        Predicates predicate = new Equal(left, right);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Equals1_ColumnCount()
    {
        Predicates left = ColumnTastyPizza;

        Predicates right = ColumnDestructCode;

        Predicates predicate = new Equal(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]

    public void Equals1_ColumnName()
    {
        Predicates left = ColumnTastyPizza;

        Predicates right = ColumnDestructCode;

        Predicates predicate = new Equal(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == $"[ColumnTable].[Pizza]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
    }

    [Fact]
    public void Equals_ToSql2()
    {
        Predicates left = ColumnFutureCity;
        string leftSql = ColumnFutureCitySql;

        Predicates right = ParameterPi;
        string rightSql = "@Pi_1";

        Predicates predicate = new Equal(left, right);

        string expectedValue = $"({leftSql} = {rightSql})";
        string actualValue = predicate.ToSqlFragments().ToSql(new SqlServerDialect());

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals2_ParameterCount()
    {
        Predicates left = ColumnFutureCity;

        Predicates right = ParameterPi;

        Predicates predicate = new Equal(left, right);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals2_ParameterValues()
    {
        Predicates left = ColumnFutureCity;

        Predicates right = ParameterPi;

        Predicates predicate = new Equal(left, right);

        float expectedValue = 3.14f;

        object? nullableActualValue = predicate.DescendantParameters.First().Value;
        Assert.NotNull(nullableActualValue);
        float actualValue = (float)nullableActualValue;

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Equals2_ColumnCount()
    {
        Predicates left = ColumnFutureCity;

        Predicates right = ParameterPi;

        Predicates predicate = new Equal(left, right);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]

    public void Equals2_ColumnName()
    {
        Predicates left = ColumnFutureCity;

        Predicates right = ParameterPi;

        Predicates predicate = new Equal(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void Equals_ToSql3()
    {
        Predicates left = ParameterElite;
        string leftSql = "@Elite_1";

        Predicates right = ParameterHelloWorld;
        string rightSql = "@HelloWorld_2";

        Predicates predicate = new Equal(left, right);

        string expectedValue = $"({leftSql} = {rightSql})";
        string actualValue = predicate.ToSqlFragments().ToSql(new SqlServerDialect());

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals3_ParameterCount()
    {
        Predicates left = ParameterElite;

        Predicates right = ParameterHelloWorld;

        Predicates predicate = new Equal(left, right);

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
    public void Equals3_ColumnCount()
    {
        Predicates left = ParameterElite;

        Predicates right = ParameterPi;

        Predicates predicate = new Equal(left, right);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals_Nested_ToSql()
    {
        Predicates left = ParameterElite;
        string leftSql = "@Elite_1";

        Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);
        string rightSql = $"(@HelloWorld_2 AND {ColumnFutureCitySql} AND {ColumnDestructCodeSql})";

        Predicates predicate = new Equal(left, right);

        string expectedValue = $"({leftSql} = {rightSql})";
        string actualValue = predicate.ToSqlFragments().ToSql(new SqlServerDialect());

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals_Nested_ParameterCount()
    {
        Predicates left = ParameterElite;

        Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        Predicates predicate = new Equal(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals_Nested_ColumnCount()
    {
        Predicates left = ParameterElite;

        Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        Predicates predicate = new Equal(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals_Nested_ParameterValue()
    {
        Predicates left = ParameterElite;

        Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        Predicates predicate = new Equal(left, right);

        int expectedValueInt = 1337;

        object? nullableActualValueInt = predicate.DescendantParameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;

        string expectedValueString = "Hello World!";
        string actualValueString = ((string?)predicate.DescendantParameters.Where(p => p.Name == "HelloWorld").First().Value) ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Equals_Nested_ColumnName()
    {
        Predicates left = ParameterElite;

        Predicates right = new And(ParameterHelloWorld, ColumnFutureCity, ColumnDestructCode);

        Predicates predicate = new Equal(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "[ColumnTable].[Express]").Single();
    }

    [Fact]
    public void Equal_LeftNull_Throws() =>
    Assert.Throws<NullReferenceException>(() => new Equal(null!, new Parameter("P1", 1, null)));

    [Fact]
    public void Equal_RightNull_Throws() =>
        Assert.Throws<NullReferenceException>(() => new Equal(new Parameter("P1", 1, null), null!));

}