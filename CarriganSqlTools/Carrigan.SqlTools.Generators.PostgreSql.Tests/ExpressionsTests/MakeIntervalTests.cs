using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class MakeIntervalTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new MakeInterval(SharedDateTimePartEnum.Day, null!)),
        (() => new MakeInterval(MakeIntervalDateTimePartEnum.Day, null!)),
    ];


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new MakeInterval(SharedDateTimePartEnum.Day, ParameterValue),
        new MakeInterval(SharedDateTimePartEnum.Day, DifferentParameter),
        new MakeInterval(SharedDateTimePartEnum.Day, ColumnA),
        new MakeInterval(SharedDateTimePartEnum.Day, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new MakeInterval(SharedDateTimePartEnum.Day, new Add(ColumnA, ParameterValue)),
        new MakeInterval(SharedDateTimePartEnum.Day, new Add(ColumnA, ParameterDifferentValue)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new MakeInterval(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new MakeInterval(SharedDateTimePartEnum.Day, ParameterValue),
        new MakeInterval(SharedDateTimePartEnum.Day, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new MakeInterval(SharedDateTimePartEnum.Day, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new MakeInterval(SharedDateTimePartEnum.Day, ParameterValue),
        new MakeInterval(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new MakeInterval(SharedDateTimePartEnum.Day, ParameterValue),
        new MakeInterval(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new MakeInterval(SharedDateTimePartEnum.Day, ColumnA),
    ];

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("MAKE_INTERVAL(days => Second)", new MakeInterval(SharedDateTimePartEnum.Day, Second).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("MAKE_INTERVAL(days => Second)", new MakeInterval(MakeIntervalDateTimePartEnum.Day, Second).ToString());
}
