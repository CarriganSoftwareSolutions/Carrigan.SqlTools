using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class XorThanTests
{
    private static readonly SqlServerDialect Dialect = new();

    private static readonly Column<ColumnTable> ColumnTastyPizza = new("Pizza");
    private static readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private static readonly Column<ColumnTable> ColumnDestructCode = new("D000destruct0");
    private static readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private static readonly Column<ColumnTable> ColumnFutureCity = new("Express");
    private static readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private static readonly Parameter ParameterPi = new(3.14f, "Pi");
    private static readonly string ParameterPiSql = "@Pi_1";

    private static readonly Parameter ParameterElite = new(1337, "Elite");
    private static readonly string ParameterEliteSql = "@Elite_1";

    private static readonly Parameter ParameterHelloWorld = new("Hello World!", "HelloWorld");
    private static readonly string ParameterHelloWorldSql = "@HelloWorld_2";


    [Fact]
    public void Xor_1_ToSql()
    {
        SqlExpression left = ColumnTastyPizza;
        string leftSql = ColumnTastyPizzaExpectedSql;

        SqlExpression right = ColumnDestructCode;
        string rightSql = ColumnDestructCodeSql;

        SqlExpression predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_1_ParameterCount()
    {
        SqlExpression left = ColumnTastyPizza;

        SqlExpression right = ColumnDestructCode;

        SqlExpression predicate = new Xor(left, right);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ToSql()
    {
        SqlExpression left = ColumnFutureCity;
        string leftSql = ColumnFutureCitySql;

        SqlExpression right = ParameterPi;
        string rightSql = ParameterPiSql;

        SqlExpression predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ParameterCount()
    {
        SqlExpression left = ColumnFutureCity;

        SqlExpression right = ParameterPi;

        SqlExpression predicate = new Xor(left, right);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ParameterValues()
    {
        SqlExpression left = ColumnFutureCity;

        SqlExpression right = ParameterPi;

        SqlExpression predicate = new Xor(left, right);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.DescendantParameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_3_ToSql()
    {
        SqlExpression left = ParameterElite;
        string leftSql = ParameterEliteSql;

        SqlExpression right = ParameterHelloWorld;
        string rightSql = ParameterHelloWorldSql;

        SqlExpression predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_3_ParameterCount()
    {
        SqlExpression left = ParameterElite;

        SqlExpression right = ParameterHelloWorld;

        SqlExpression predicate = new Xor(left, right);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.DescendantParameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.DescendantParameters.First(p => p.Name == "HelloWorld").Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Xor_Nested_ToSql()
    {
        SqlExpression left = ParameterElite;
        string leftSql = ParameterEliteSql;

        SqlExpression right = new And(new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));
        string rightSql = $"(({ParameterHelloWorldSql} IS NOT NULL) AND ({ColumnFutureCitySql} IS NOT NULL) AND ({ColumnDestructCodeSql} IS NOT NULL))";

        SqlExpression predicate = new Xor(left, right);

        string expectedValue = $"({leftSql} ^ {rightSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ParameterCount()
    {
        SqlExpression left = ParameterElite;

        SqlExpression right = new And(new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        SqlExpression predicate = new Xor(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ParameterValue()
    {
        SqlExpression left = ParameterElite;

        SqlExpression right = new And(new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        SqlExpression predicate = new Xor(left, right);

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
    public void Xor_1_ColumnCount()
    {
        SqlExpression left = ColumnTastyPizza;

        SqlExpression right = ColumnDestructCode;

        SqlExpression predicate = new Xor(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_1_ColumnName()
    {
        SqlExpression left = ColumnTastyPizza;

        SqlExpression right = ColumnDestructCode;

        SqlExpression predicate = new Xor(left, right);


        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Pizza]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Pizza").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.D000destruct0").Single();
    }

    [Fact]
    public void Xor_2_ColumnCount()
    {
        SqlExpression left = ColumnFutureCity;

        SqlExpression right = ParameterPi;

        SqlExpression predicate = new Xor(left, right);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_2_ColumnName()
    {
        SqlExpression left = ColumnFutureCity;

        SqlExpression right = ParameterPi;

        SqlExpression predicate = new Xor(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Express]").Single();

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Express").Single();
    }

    [Fact]
    public void Xor_3_ColumnCount()
    {
        SqlExpression left = ParameterElite;

        SqlExpression right = ParameterPi;

        SqlExpression predicate = new Xor(left, right);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ColumnCount()
    {
        SqlExpression left = ParameterElite;

        Predicates right = new And(new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        SqlExpression predicate = new Xor(left, right);

        int expectedValue = 2;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Xor_Nested_ColumnName()
    {
        SqlExpression left = ParameterElite;

        SqlExpression right = new And(new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        SqlExpression predicate = new Xor(left, right);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Express]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.D000destruct0").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Express").Single();
    }
}