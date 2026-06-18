using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class BooleanColumnTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void Constructor_BoolColumn()
    {
        BooleanColumn<LogicalPredicateTable> booleanColumn = new(nameof(LogicalPredicateTable.IsActive));

        string expectedValue = "[LogicalPredicateTable].[IsActive]";
        string actualValue = booleanColumn.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Constructor_NullableBoolColumn()
    {
        BooleanColumn<LogicalPredicateTable> booleanColumn = new(nameof(LogicalPredicateTable.IsVisible));

        string expectedValue = "[LogicalPredicateTable].[IsVisible]";
        string actualValue = booleanColumn.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void Constructor_NonBooleanColumn_Exception() =>
        Assert.Throws<ArgumentException>(() => new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.Name)));
}
