using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class OrTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void Or_Empty_ToSql() =>
        Assert.Throws<ArgumentException>(() => new Or([]));

    [Fact]
    public void Or_Null_ToSql() =>
        Assert.Throws<ArgumentNullException>(() => new Or(null!));

    [Fact]
    public void Or_Single_ToSql()
    {
        Or or = new(
        [
            new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsActive)),
        ]);

        string expected = "[LogicalPredicateTable].[IsActive]";
        string actual = or.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Or_ToSql()
    {
        Or or = CreateOr();

        string expected = "([LogicalPredicateTable].[IsActive] OR (@P1_1 IS NOT NULL) OR (@P2_2 IS NOT NULL) OR [LogicalPredicateTable].[IsEnabled] OR ([LogicalPredicateTable].[IsVisible] AND [LogicalPredicateTable].[IsArchived] AND (@PA_3 IS NOT NULL)))";
        string actual = or.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Or_ParameterCount()
    {
        Or or = CreateOr();

        int actual = or.DescendantParameters.Count();
        int expected = 3;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Or_ParameterValues()
    {
        Or or = CreateOr(3);

        Parameter parameter = or.DescendantParameters.Where(parameter => parameter.Name == "P1").Single();
        object? nullableActual = parameter.Value;
        Assert.NotNull(nullableActual);
        int actual = (int)nullableActual;
        int expected = 1;

        Assert.Equal(expected, actual);

        parameter = or.DescendantParameters.Where(parameter => parameter.Name == "P2").Single();
        nullableActual = parameter.Value;
        Assert.NotNull(nullableActual);
        actual = (int)nullableActual;
        expected = 2;

        Assert.Equal(expected, actual);

        parameter = or.DescendantParameters.Where(parameter => parameter.Name == "PA").Single();
        nullableActual = parameter.Value;
        Assert.NotNull(nullableActual);
        actual = (int)nullableActual;
        expected = 3;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Or_ColumnsCount()
    {
        Or or = CreateOr();

        int actual = or.DescendantColumns.Count();
        int expected = 4;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Or_ColumnName()
    {
        Or or = CreateOr();

        _ = or.DescendantColumns.Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == "[LogicalPredicateTable].[IsActive]").Single();
        _ = or.DescendantColumns.Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == "[LogicalPredicateTable].[IsEnabled]").Single();
        _ = or.DescendantColumns.Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == "[LogicalPredicateTable].[IsVisible]").Single();
        _ = or.DescendantColumns.Where(column => column.ColumnInfo.ColumnTag.ToSql(Dialect) == "[LogicalPredicateTable].[IsArchived]").Single();
        _ = or.DescendantColumns.Where(column => column.ColumnInfo == "LogicalPredicateTable.IsActive").Single();
        _ = or.DescendantColumns.Where(column => column.ColumnInfo == "LogicalPredicateTable.IsEnabled").Single();
        _ = or.DescendantColumns.Where(column => column.ColumnInfo == "LogicalPredicateTable.IsVisible").Single();
        _ = or.DescendantColumns.Where(column => column.ColumnInfo == "LogicalPredicateTable.IsArchived").Single();
    }

    [Fact]
    public void Or_ContainsNullPredicate_ThrowsNullReferenceException() =>
        Assert.Throws<NullReferenceException>(() => new Or(
        [
            new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsActive)),
            null!,
        ]));

    private static Or CreateOr(int nestedParameterValue = 2) =>
        new(
        [
            new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsActive)),
            new IsNotNull(new Parameter(1, "P1")),
            new IsNotNull(new Parameter(2, "P2")),
            new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsEnabled)),
            new And(
            [
                new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsVisible)),
                new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsArchived)),
                new IsNotNull(new Parameter(nestedParameterValue, "PA")),
            ]),
        ]);
}
