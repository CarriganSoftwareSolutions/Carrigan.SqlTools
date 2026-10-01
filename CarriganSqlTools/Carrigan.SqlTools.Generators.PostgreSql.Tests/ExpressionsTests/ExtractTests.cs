using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class ExtractTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Extract(SharedDateTimePartEnum.Day, null!)),
        (() => new Extract(ExtractDateTimePartEnum.Epoch, null!)),
    ];


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Extract(SharedDateTimePartEnum.Day, ParameterValue),
        new Extract(SharedDateTimePartEnum.Day, DifferentParameter),
        new Extract(SharedDateTimePartEnum.Day, ColumnA),
        new Extract(SharedDateTimePartEnum.Day, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Extract(SharedDateTimePartEnum.Day, new Add(ColumnA, ParameterValue)),
        new Extract(SharedDateTimePartEnum.Day, new Add(ColumnA, ParameterDifferentValue)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new Extract(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Extract(SharedDateTimePartEnum.Day, ParameterValue),
        new Extract(SharedDateTimePartEnum.Day, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Extract(SharedDateTimePartEnum.Day, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Extract(SharedDateTimePartEnum.Day, ParameterValue),
        new Extract(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Extract(SharedDateTimePartEnum.Day, ParameterValue),
        new Extract(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Extract(SharedDateTimePartEnum.Day, ColumnA),
    ];

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("EXTRACT(day FROM Value)", new Extract(SharedDateTimePartEnum.Day, DateValue).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("EXTRACT(epoch FROM Value)", new Extract(ExtractDateTimePartEnum.Epoch, Value).ToString());
}
