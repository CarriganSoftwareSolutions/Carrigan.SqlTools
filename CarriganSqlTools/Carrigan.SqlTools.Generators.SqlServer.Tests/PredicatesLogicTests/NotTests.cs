using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class NotTests
{
    private static readonly SqlServerDialect Dialect = new();

    private readonly BooleanColumn<LogicalPredicateTable> ColumnIsActive = new(nameof(LogicalPredicateTable.IsActive));
    private readonly string ColumnIsActiveSql = "[LogicalPredicateTable].[IsActive]";


    private readonly BooleanColumn<LogicalPredicateTable> ColumnIsVisible = new(nameof(LogicalPredicateTable.IsVisible));
    private readonly string ColumnIsVisibleSql = "[LogicalPredicateTable].[IsVisible]";

    private readonly BooleanColumn<LogicalPredicateTable> ColumnIsArchived = new(nameof(LogicalPredicateTable.IsArchived));
    private readonly string ColumnIsArchivedSql = "[LogicalPredicateTable].[IsArchived]";

    private readonly Parameter ParameterPi = new(3.14f, "Pi");
    private readonly string ParameterPiSql = "@Pi_1";

    private readonly Parameter ParameterElite = new(1337, "Elite");
    private readonly string ParameterEliteSql = "@Elite_1";

    private readonly Parameter ParameterHelloWorld = new("Hello World!", "HelloWorld");
    private readonly string ParameterHelloWorldSql = "@HelloWorld_2";

    [Fact]
    public void Not_BooleanColumn_ToSql()
    {
        Predicates inner = ColumnIsActive;
        string innerSql = ColumnIsActiveSql;

        Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_BooleanColumn_ParameterCount()
    {
        Predicates predicate = new Not(ColumnIsActive);

        int expectedValue = 0;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_BooleanColumn_ColumnCount()
    {
        Predicates predicate = new Not(ColumnIsActive);

        int expectedValue = 1;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_BooleanColumn_ColumnName()
    {
        Predicates predicate = new Not(ColumnIsActive);

        _ = predicate.DescendantColumns.Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == ColumnIsActiveSql).Single();
        _ = predicate.DescendantColumns.Where(column => column.ColumnInfo == "LogicalPredicateTable.IsActive").Single();
    }

    [Fact]
    public void Not_IsNotNullParameter_ToSql()
    {
        Predicates inner = new IsNotNull(ParameterPi);
        string innerSql = $"({ParameterPiSql} IS NOT NULL)";

        Predicates predicate = new Not(inner);

        string expectedValue = $"(NOT {innerSql})";
        string actualValue = predicate.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_IsNotNullParameter_ParameterCount()
    {
        Predicates predicate = new Not(new IsNotNull(ParameterPi));

        int expectedValue = 1;
        int actualValue = predicate.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_IsNotNullParameter_ParameterValues()
    {
        Predicates predicate = new Not(new IsNotNull(ParameterPi));

        float expectedValue = 3.14f;
        object? nullableActualValueFloat = predicate.DescendantParameters.First().Value;
        Assert.NotNull(nullableActualValueFloat);
        float actualValue = (float)nullableActualValueFloat;

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_IsNotNullParameter_ColumnCount()
    {
        Predicates predicate = new Not(new IsNotNull(ParameterPi));

        int expectedValue = 0;
        int actualValue = predicate.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ToSql()
    {
        Predicates and = CreateNestedPredicate();
        string andSql = $"((NOT ({ParameterEliteSql} IS NOT NULL)) AND (NOT ({ParameterHelloWorldSql} IS NOT NULL)) AND (NOT {ColumnIsVisibleSql}) AND (NOT {ColumnIsArchivedSql}))";

        string expectedValue = andSql;
        string actualValue = and.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ParameterCount()
    {
        Predicates and = CreateNestedPredicate();

        int expectedValue = 2;
        int actualValue = and.DescendantParameters.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ParameterValue()
    {
        Predicates and = CreateNestedPredicate();

        int expectedValueInt = 1337;
        object? nullableActualValueInt = and.DescendantParameters.Where(parameter => parameter.Name == "Elite").First().Value;
        Assert.NotNull(nullableActualValueInt);
        int actualValueInt = (int)nullableActualValueInt;
        string expectedValueString = "Hello World!";
        string actualValueString = (string?)and.DescendantParameters.First(parameter => parameter.Name == "HelloWorld").Value ?? string.Empty;

        Assert.Equal(expectedValueInt, actualValueInt);
        Assert.Equal(expectedValueString, actualValueString);
    }

    [Fact]
    public void Not_Nested_ColumnCount()
    {
        Predicates and = CreateNestedPredicate();

        int expectedValue = 2;
        int actualValue = and.DescendantColumns.Count();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Not_Nested_ColumnName()
    {
        Predicates and = CreateNestedPredicate();

        _ = and.DescendantColumns.Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == ColumnIsVisibleSql).Single();
        _ = and.DescendantColumns.Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == ColumnIsArchivedSql).Single();
        _ = and.DescendantColumns.Where(column => column.ColumnInfo == "LogicalPredicateTable.IsVisible").Single();
        _ = and.DescendantColumns.Where(column => column.ColumnInfo == "LogicalPredicateTable.IsArchived").Single();
    }

    [Fact]
    public void Not_NullPredicate_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new Not(null!));

    private And CreateNestedPredicate() =>
        new (new Not(new IsNotNull(ParameterElite)), new Not(new IsNotNull(ParameterHelloWorld)), new Not(ColumnIsVisible), new Not(ColumnIsArchived));
}
