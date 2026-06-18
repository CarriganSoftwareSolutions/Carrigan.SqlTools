using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class AndTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void And_Empty_ToSql() =>
        Assert.Throws<ArgumentException>(() => new And([]));

    [Fact]
    public void And_Null_ToSql() =>
        Assert.Throws<ArgumentNullException>(() => new And(null!));

    [Fact]
    public void And_Single_ToSql()
    {
        And and = new(
        [
            new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsActive)),
        ]);

        string expected = "[LogicalPredicateTable].[IsActive]";
        string actual = and.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ToSql()
    {
        And and = CreateAnd();

        string expected = "([LogicalPredicateTable].[IsActive] AND (@P1_1 IS NOT NULL) AND (@P2_2 IS NOT NULL) AND [LogicalPredicateTable].[IsEnabled] AND ([LogicalPredicateTable].[IsVisible] OR [LogicalPredicateTable].[IsArchived] OR (@PA_3 IS NOT NULL)))";
        string actual = and.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ParameterCount()
    {
        And and = CreateAnd();

        int actual = and.DescendantParameters.Count();
        int expected = 3;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ColumnsCount()
    {
        And and = CreateAnd();

        int actual = and.DescendantColumns.Count();
        int expected = 4;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ParameterValues()
    {
        And and = CreateAnd(3);

        Parameter parameter = and.DescendantParameters.Where(parameter => parameter.Name == "P1").Single();
        Assert.NotNull(parameter.Value);
        int actual = (int)parameter.Value;
        int expected = 1;

        Assert.Equal(expected, actual);

        parameter = and.DescendantParameters.Where(parameter => parameter.Name == "P2").Single();
        Assert.NotNull(parameter.Value);
        actual = (int)parameter.Value;
        expected = 2;

        Assert.Equal(expected, actual);

        parameter = and.DescendantParameters.Where(parameter => parameter.Name == "PA").Single();
        Assert.NotNull(parameter.Value);
        actual = (int)parameter.Value;
        expected = 3;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void And_ColumnName()
    {
        And and = CreateAnd();

        _ = and.DescendantColumns.Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == "[LogicalPredicateTable].[IsActive]").Single();
        _ = and.DescendantColumns.Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == "[LogicalPredicateTable].[IsEnabled]").Single();
        _ = and.DescendantColumns.Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == "[LogicalPredicateTable].[IsVisible]").Single();
        _ = and.DescendantColumns.Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == "[LogicalPredicateTable].[IsArchived]").Single();
        _ = and.DescendantColumns.Where(column => column.ColumnInfo.ColumnTag == "LogicalPredicateTable.IsArchived").Single();
        _ = and.DescendantColumns.Where(column => column.ColumnInfo == "LogicalPredicateTable.IsArchived").Single();
    }

    [Fact]
    public void And_ContainsNullPredicate_ThrowsNullReferenceException() =>
        Assert.Throws<NullReferenceException>(() => new And(
        [
            new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsActive)),
            null!,
        ]));

    private static And CreateAnd(int nestedParameterValue = 2) =>
        new(
        [
            new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsActive)),
            new IsNotNull(new Parameter(1, "P1")),
            new IsNotNull(new Parameter(2, "P2")),
            new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsEnabled)),
            new Or(
            [
                new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsVisible)),
                new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsArchived)),
                new IsNotNull(new Parameter(nestedParameterValue, "PA")),
            ]),
        ]);
}
