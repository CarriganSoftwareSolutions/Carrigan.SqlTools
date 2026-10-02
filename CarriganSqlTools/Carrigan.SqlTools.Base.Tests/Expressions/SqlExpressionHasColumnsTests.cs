using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

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
    public void HasColumns_NestedColumnTagExpression_ReturnsTrue()
    {
        ColumnTagExpression column = new(new ColumnTag(new TableTag(null, "TestTable"), new ColumnName("Value")));

        Assert.True(new WrapperExpression(column).HasColumns());
    }

    [Fact]
    public void HasColumns_WithoutColumn_ReturnsFalse() =>
        Assert.False(new WrapperExpression(new Parameter(1)).HasColumns());
    private sealed class ParticipatingTableExpression : SqlExpression
    {
        private readonly TableTag _tableTag;

        internal ParticipatingTableExpression(string tableName, params SqlExpression[] children) : base(children) =>
            _tableTag = new TableTag(null, tableName);

        public override IEnumerable<TableTag> LeafTables => [_tableTag];

        public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect) => [];
    }

    [Fact]
    public void AllParticipatingColumns_IncludesCurrentNode()
    {
        ColumnTagExpression column = new(new ColumnTag(new TableTag(null, "TestTable"), new ColumnName("Value")));

        Assert.Same(column, Assert.Single(column.AllParticipatingColumns));
    }

    [Fact]
    public void AllParticipatingColumns_IncludesColumnsInsideAggregateSubtrees()
    {
        ColumnTagExpression column = new(new ColumnTag(new TableTag(null, "TestTable"), new ColumnName("Value")));
        Count count = new(column);

        Assert.Same(column, Assert.Single(count.AllParticipatingColumns));
    }

    [Fact]
    public void AllNonAggregateColumns_StopsAtAggregateSubtrees()
    {
        ColumnTagExpression groupedColumn = new(new ColumnTag(new TableTag(null, "TestTable"), new ColumnName("Grouped")));
        ColumnTagExpression aggregateColumn = new(new ColumnTag(new TableTag(null, "TestTable"), new ColumnName("Aggregated")));
        Add expression = new(groupedColumn, new Sum(aggregateColumn));

        Assert.Same(groupedColumn, Assert.Single(expression.AllNonAggregateColumns));
    }

    [Fact]
    public void AllParticipatingParameters_IncludesCurrentNode() =>
        Assert.Single(new Parameter(1, "Value").AllParticipatingParameters);

    [Fact]
    public void AllParticipatingParameters_IncludesDescendants()
    {
        Parameter parameter = new(1, "Value");
        WrapperExpression expression = new(parameter);

        Assert.Same(parameter, Assert.Single(expression.AllParticipatingParameters));
    }

    [Fact]
    public void AllParticipatingTables_IncludesCurrentAndChildTables()
    {
        ParticipatingTableExpression child = new("Child");
        ParticipatingTableExpression expression = new("Parent", child);

        Assert.Equal(["Parent", "Child"], expression.AllParticipatingTables.Select(static table => table.ToString()));
    }

}
