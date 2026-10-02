using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class CoalesceTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Coalesce(null!, Value)),
        (() => new Coalesce(Value, null!)),
        (() => new Coalesce(null!, Value, First)),
        (() => new Coalesce(Value, null!, First)),
        (() => new Coalesce(Value, First, null!)),
    ];



    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new Coalesce(AggregateWithParameterNoColumn, ColumnA)),
        (() => new Coalesce(ColumnA, AggregateWithParameterNoColumn)),
        (() => new Coalesce(AggregateWithParameterNoColumn, Second, ColumnA)),
        (() => new Coalesce(ColumnA, Second, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Coalesce(First, Second),
        new Coalesce(First, Third),
        new Coalesce(Second, Third),
        new Coalesce(ColumnA, ParameterValue),
        new Coalesce(ColumnB, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Coalesce(ColumnA, ParameterValue),
        new Coalesce(ColumnA, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new Coalesce(AggregateWithParameterNoColumn, Second),
        new Coalesce(First, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Coalesce(First, Second),
        new Coalesce(ColumnA, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Coalesce(ColumnA, Second),
        new Coalesce(First, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Coalesce(First, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Coalesce(First, Second),
        new Coalesce(ColumnA, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Coalesce(ColumnA, ColumnB),
    ];

    [Fact]
    public void AllParticipatingTables_ContainsNestedColumnTable()
    {
        Column<Customer> customerId = new(nameof(Customer.Id));
        Add nestedExpression = new(customerId, new Parameter(1, "Offset"));
        Coalesce coalesce = new(nestedExpression, new Parameter(0, "Fallback"));
        TableTag expected = customerId.ColumnInfo.ColumnTag.TableTag;

        TableTag actual = Assert.Single(coalesce.AllParticipatingTables);

        Assert.Equal(expected, actual);
    }
}
