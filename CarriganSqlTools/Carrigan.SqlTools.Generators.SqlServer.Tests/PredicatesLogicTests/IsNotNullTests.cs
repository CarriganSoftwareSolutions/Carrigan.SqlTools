using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class IsNotNullTests
{
    private static readonly SqlServerDialect Dialect = new();

    private readonly Column<ColumnTable> ColumnTastyPizza = new("Pizza");
    private readonly string ColumnTastyPizzaExpectedSql = "[ColumnTable].[Pizza]";

    private readonly Column<ColumnTable> ColumnDestructCode = new("D000destruct0");
    private readonly string ColumnDestructCodeSql = "[ColumnTable].[D000destruct0]";

    private readonly Column<ColumnTable> ColumnFutureCity = new("Express");
    private readonly string ColumnFutureCitySql = "[ColumnTable].[Express]";

    private readonly Parameter ParameterPi = new(3.14f, "Pi");
    private readonly string ParameterPiSql = "@Pi_1";

    private readonly Parameter ParameterElite = new(1337, "Elite");
    private readonly string ParameterEliteSql = "@Elite_1";

    private readonly Parameter ParameterHelloWorld = new("Hello World!", "HelloWorld");
    private readonly string ParameterHelloWorldSql = "@HelloWorld_1";


    [Fact]
    public void IsNotNull_1_ToSql()
    {
        SqlExpression inner = ColumnTastyPizza;
        string innerSql = ColumnTastyPizzaExpectedSql;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        string expectedValue = $"({innerSql} IS NOT NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void IsNotNull_1_ParameterCount()
    {
        SqlExpression inner = ColumnTastyPizza;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_2_ToSql()
    {
        SqlExpression inner = ColumnDestructCode;
        string innerSql = ColumnDestructCodeSql;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        string expectedValue = $"({innerSql} IS NOT NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void IsNotNull_2_ParameterCount()
    {
        SqlExpression inner = ColumnDestructCode;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_3_ToSql()
    {
        SqlExpression inner = ColumnFutureCity;
        string innerSql = ColumnFutureCitySql;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        string expectedValue = $"({innerSql} IS NOT NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_3_ParameterCount()
    {
        SqlExpression inner = ColumnFutureCity;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_4_ToSql()
    {
        SqlExpression inner = ParameterPi;
        string innerSql = ParameterPiSql;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        string expectedValue = $"({innerSql} IS NOT NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_4_ParameterCount()
    {
        SqlExpression inner = ParameterPi;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_4_ParameterValues()
    {
        SqlExpression inner = ParameterPi;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.DescendantParameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }


    [Fact]
    public void IsNotNull_5_ToSql()
    {
        SqlExpression inner = ParameterElite;
        string innerSql = ParameterEliteSql;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        string expectedValue = $"({innerSql} IS NOT NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_5_ParameterCount()
    {
        SqlExpression inner = ParameterElite;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_5_ParameterValue()
    {
        SqlExpression inner = ParameterElite;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValueInt = 1337;
        object? nullableActualValueInt = predicate.DescendantParameters.Where(p => p.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        Assert.Equal(expectedValueInt, actualValueInt);
    }


    [Fact]
    public void IsNotNull_6_ToSql()
    {
        SqlExpression inner = ParameterHelloWorld;
        string innerSql = ParameterHelloWorldSql;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        string expectedValue = $"({innerSql} IS NOT NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_6_ParameterCount()
    {
        SqlExpression inner = ParameterHelloWorld;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_6_ParameterValue()
    {

        SqlExpression inner = ParameterHelloWorld;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        string expectedValueString = "Hello World!";
        string actualValueString = (string?)predicate.DescendantParameters.Where(p => p.Name == "HelloWorld").First().Value ?? string.Empty;

        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void IsNotNull_Nested_ToSql()
    {
        PredicatesLogic.Predicates and = new And(new IsNotNull(ParameterElite), new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));
        string andSql = $"((@Elite_1 IS NOT NULL) AND (@HelloWorld_2 IS NOT NULL) AND ({ColumnFutureCitySql} IS NOT NULL) AND ({ColumnDestructCodeSql} IS NOT NULL))";

        string expectedValue = andSql;
        string actualValue = and.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_Nested_ParameterCount()
    {
        PredicatesLogic.Predicates and = new And(new IsNotNull(ParameterElite), new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_Nested_ParameterValue()
    {
        PredicatesLogic.Predicates and = new And(new IsNotNull(ParameterElite), new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

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
    public void IsNotNull_1_ColumnCount()
    {
        SqlExpression inner = ColumnTastyPizza;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_1_ColumnName()
    {
        SqlExpression inner = ColumnTastyPizza;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);


        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Pizza]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Pizza").Single();
    }

    [Fact]
    public void IsNotNull_2_ColumnCount()
    {
        SqlExpression inner = ColumnDestructCode;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_2_ColumnName()
    {
        SqlExpression inner = ColumnDestructCode;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.D000destruct0").Single();
    }

    [Fact]
    public void IsNotNull_3_ColumnCount()
    {
        SqlExpression inner = ColumnFutureCity;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_3_ColumnName()
    {
        SqlExpression inner = ColumnFutureCity;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Express]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Express").Single();
    }

    [Fact]
    public void IsNotNull_4_ColumnCount()
    {
        SqlExpression inner = ParameterPi;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_5_ColumnCount()
    {
        SqlExpression inner = ParameterElite;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_6_ColumnCount()
    {
        SqlExpression inner = ParameterHelloWorld;

        PredicatesLogic.Predicates predicate = new IsNotNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_Nested_ColumnCount()
    {
        PredicatesLogic.Predicates and = new And(new IsNotNull(ParameterElite), new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        int expectedValue = 2;
        int actualValue = and.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNotNull_Nested_ColumnName()
    {
        PredicatesLogic.Predicates and = new And(new IsNotNull(ParameterElite), new IsNotNull(ParameterHelloWorld), new IsNotNull(ColumnFutureCity), new IsNotNull(ColumnDestructCode));

        _ = and.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[D000destruct0]").Single();
        _ = and.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Express]").Single();
        _ = and.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.D000destruct0").Single();
        _ = and.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Express").Single();
    }

    [Fact]
    public void IsNotNull_NullValue_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new IsNotNull(null!));

}