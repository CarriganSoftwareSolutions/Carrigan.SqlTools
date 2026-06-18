using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class EqualsTests
{
    private static readonly SqlServerDialect Dialect = new();

    private readonly Column<ColumnTable> ColumnTastyPizza = new("Pizza");
    private readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private readonly Column<ColumnTable> ColumnDestructCode = new("D000destruct0");
    private readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private readonly Column<ColumnTable> ColumnFutureCity = new("Express");
    private readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private readonly Parameter ParameterPi = new(3.14f, "Pi");

    private readonly Parameter ParameterElite = new(1337, "Elite");

    private readonly Parameter ParameterHelloWorld = new("Hello World!", "HelloWorld");


    [Fact]
    public void Equals_ToSql1()
    {
        Column<ColumnTable> left = ColumnTastyPizza;
        string leftSql = ColumnTastyPizzaExpectedSql;

        Column<ColumnTable> right = ColumnDestructCode;
        string rightSql = ColumnDestructCodeSql;

        Predicates predicate = new Equal(left, right);

        string expectedValue = $"({leftSql} = {rightSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Equals1_ParameterCount()
    {
        Column<ColumnTable> left = ColumnTastyPizza;

        Column<ColumnTable> right = ColumnDestructCode;

        Predicates predicate = new Equal(left, right);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void Equals1_ColumnCount()
    {
        Column<ColumnTable> left = ColumnTastyPizza;

        Column<ColumnTable> right = ColumnDestructCode;

        Predicates predicate = new Equal(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]

    public void Equals1_ColumnName()
    {
        Column<ColumnTable> left = ColumnTastyPizza;

        Column<ColumnTable> right = ColumnDestructCode;

        Predicates predicate = new Equal(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Pizza]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.D000destruct0").Single();
    }

    [Fact]
    public void Equals_ToSql2()
    {
        Column<ColumnTable> left = ColumnFutureCity;
        string leftSql = ColumnFutureCitySql;

        Parameter right = ParameterPi;
        string rightSql = "@Pi_1";

        Predicates predicate = new Equal(left, right);

        string expectedValue = $"({leftSql} = {rightSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals2_ParameterCount()
    {
        Column<ColumnTable> left = ColumnFutureCity;

        Parameter right = ParameterPi;

        Predicates predicate = new Equal(left, right);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals2_ParameterValues()
    {
        Column<ColumnTable> left = ColumnFutureCity;

        Parameter right = ParameterPi;

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
        Column<ColumnTable> left = ColumnFutureCity;

        Parameter right = ParameterPi;

        Predicates predicate = new Equal(left, right);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]

    public void Equals2_ColumnName()
    {
        Column<ColumnTable> left = ColumnFutureCity;

        Parameter right = ParameterPi;

        Predicates predicate = new Equal(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Express]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag == "ColumnTable.Express").Single();
    }

    [Fact]
    public void Equals_ToSql3()
    {
        Parameter left = ParameterElite;
        string leftSql = "@Elite_1";

        Parameter right = ParameterHelloWorld;
        string rightSql = "@HelloWorld_2";

        Predicates predicate = new Equal(left, right);

        string expectedValue = $"({leftSql} = {rightSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals3_ParameterCount()
    {
        Parameter left = ParameterElite;

        Parameter right = ParameterHelloWorld;

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
        Parameter left = ParameterElite;

        Parameter right = ParameterPi;

        Predicates predicate = new Equal(left, right);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals_Nested_ToSql()
    {
        Parameter left = ParameterElite;
        string leftSql = "@Elite_1";

        And right = new(new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));
        string rightSql = $"((@HelloWorld_2 IS NOT NULL) AND ({ColumnFutureCitySql} IS NOT NULL) AND ({ColumnDestructCodeSql} IS NOT NULL))";

        Predicates predicate = new Equal(left, right);

        string expectedValue = $"({leftSql} = {rightSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals_Nested_ParameterCount()
    {
        Parameter left = ParameterElite;

        And right = new(new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        Predicates predicate = new Equal(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals_Nested_ColumnCount()
    {
        Parameter left = ParameterElite;

        And right = new(new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        Predicates predicate = new Equal(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Equals_Nested_ParameterValue()
    {
        Parameter left = ParameterElite;

        And right = new(new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

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
        Parameter left = ParameterElite;

        And right = new(new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        Predicates predicate = new Equal(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Express").Single();
    }

    [Fact]
    public void Equal_LeftNull_Throws() =>
    Assert.Throws<NullReferenceException>(() => new Equal(null!, new Parameter(1, "P1")));

    [Fact]
    public void Equal_RightNull_Throws() =>
        Assert.Throws<NullReferenceException>(() => new Equal(new Parameter(1, "P1"), null!));

}