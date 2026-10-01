using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

//IGNORE SPELLING: Trunc

public class DateTruncTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new DateTrunc(SharedDateTimePartEnum.Day, null!)),
        (() => new DateTrunc(DateTruncDateTimePartEnum.Quarter, null!)),
    ];




    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new DateTrunc(SharedDateTimePartEnum.Day, ParameterValue),
        new DateTrunc(SharedDateTimePartEnum.Day, DifferentParameter),
        new DateTrunc(SharedDateTimePartEnum.Day, ColumnA),
        new DateTrunc(SharedDateTimePartEnum.Day, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new DateTrunc(SharedDateTimePartEnum.Day, new Add(ColumnA, ParameterValue)),
        new DateTrunc(SharedDateTimePartEnum.Day, new Add(ColumnA, ParameterDifferentValue)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new DateTrunc(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new DateTrunc(SharedDateTimePartEnum.Day, ParameterValue),
        new DateTrunc(SharedDateTimePartEnum.Day, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new DateTrunc(SharedDateTimePartEnum.Day, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new DateTrunc(SharedDateTimePartEnum.Day, ParameterValue),
        new DateTrunc(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new DateTrunc(SharedDateTimePartEnum.Day, ParameterValue),
        new DateTrunc(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new DateTrunc(SharedDateTimePartEnum.Day, ColumnA),
    ];

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("DATE_TRUNC('day', Value)", new DateTrunc(SharedDateTimePartEnum.Day, DateValue).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("DATE_TRUNC('quarter', Value)", new DateTrunc(DateTruncDateTimePartEnum.Quarter, Value).ToString());
}
