using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class IsNullTests
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
    public void IsNull_1_ToSql()
    {
        SqlExpression inner = ColumnTastyPizza;
        string innerSql = ColumnTastyPizzaExpectedSql;

        Predicates predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void IsNull_1_ParameterCount()
    {
        SqlExpression inner = ColumnTastyPizza;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_2_ToSql()
    {
        SqlExpression inner = ColumnDestructCode;
        string innerSql = ColumnDestructCodeSql;

        Predicates predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }
    [Fact]
    public void IsNull_2_ParameterCount()
    {
        SqlExpression inner = ColumnDestructCode;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_3_ToSql()
    {
        SqlExpression inner = ColumnFutureCity;
        string innerSql = ColumnFutureCitySql;

        Predicates predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_3_ParameterCount()
    {
        SqlExpression inner = ColumnFutureCity;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_4_ToSql()
    {
        SqlExpression inner = ParameterPi;
        string innerSql = ParameterPiSql;

        Predicates predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_4_ParameterCount()
    {
        SqlExpression inner = ParameterPi;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_4_ParameterValues()
    {
        SqlExpression inner = ParameterPi;

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
        SqlExpression inner = ParameterElite;
        string innerSql = ParameterEliteSql;

        Predicates predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_5_ParameterCount()
    {
        SqlExpression inner = ParameterElite;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_5_ParameterValue()
    {
        SqlExpression inner = ParameterElite;

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
        SqlExpression inner = ParameterHelloWorld;
        string innerSql = ParameterHelloWorldSql;

        Predicates predicate = new IsNull(inner);

        string expectedValue = $"({innerSql} IS NULL)";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_6_ParameterCount()
    {
        SqlExpression inner = ParameterHelloWorld;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_6_ParameterValue()
    {

        SqlExpression inner = ParameterHelloWorld;

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
        SqlExpression inner = ColumnTastyPizza;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_1_ColumnName()
    {
        SqlExpression inner = ColumnTastyPizza;

        Predicates predicate = new IsNull(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Pizza]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Pizza").Single();
    }

    [Fact]
    public void IsNull_2_ColumnCount()
    {
        SqlExpression inner = ColumnDestructCode;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_2_ColumnName()
    {
        SqlExpression inner = ColumnDestructCode;

        Predicates predicate = new IsNull(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[D000destruct0]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.D000destruct0").Single();
    }

    [Fact]
    public void IsNull_3_ColumnCount()
    {
        SqlExpression inner = ColumnFutureCity;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_3_ColumnName()
    {
        SqlExpression inner = ColumnFutureCity;

        Predicates predicate = new IsNull(inner);

        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo.ColumnTag.ToSql(Dialect) == "[ColumnTable].[Express]").Single();
        _ = predicate.DescendantColumns.Where(col => col.ColumnInfo == "ColumnTable.Express").Single();
    }

    [Fact]
    public void IsNull_4_ColumnCount()
    {
        SqlExpression inner = ParameterPi;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_5_ColumnCount()
    {
        SqlExpression inner = ParameterElite;

        Predicates predicate = new IsNull(inner);

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void IsNull_6_ColumnCount()
    {
        SqlExpression inner = ParameterHelloWorld;

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