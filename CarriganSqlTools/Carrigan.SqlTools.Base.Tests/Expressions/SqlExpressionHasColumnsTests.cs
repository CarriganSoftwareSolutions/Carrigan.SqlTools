using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

internal sealed class TestColumnExpression : SqlExpression
{
    internal TestColumnExpression() : base([])
    {
    }

    protected override bool IsColumn() =>
        true;

    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect) =>
        [];
}

public class SqlExpressionHasColumnsTests
{
    private sealed class WrapperExpression : SqlExpression
    {
        internal WrapperExpression(SqlExpression child) : base([child])
        {
        }

        public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect) =>
            [];
    }

    [Fact]
    public void HasColumns_ColumnExpression_ReturnsTrue() =>
        Assert.True(new TestColumnExpression().HasColumns());

    [Fact]
    public void HasColumns_NestedColumnExpression_ReturnsTrue() =>
        Assert.True(new WrapperExpression(new TestColumnExpression()).HasColumns());

    [Fact]
    public void HasColumns_NestedColumnTagExpression_ReturnsTrue()
    {
        ColumnTagExpression column = new(new ColumnTag(new TableTag(null, "TestTable"), new ColumnName("Value")));

        Assert.True(new WrapperExpression(column).HasColumns());
    }

    [Fact]
    public void HasColumns_WithoutColumn_ReturnsFalse() =>
        Assert.False(new WrapperExpression(new Parameter(1)).HasColumns());
}
