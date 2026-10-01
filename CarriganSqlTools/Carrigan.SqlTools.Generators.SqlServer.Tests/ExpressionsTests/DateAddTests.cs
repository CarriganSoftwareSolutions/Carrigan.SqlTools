using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class DateAddTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new DateAdd(SharedDateTimePartEnum.Day, null!, Value)),
        (() => new DateAdd(SharedDateTimePartEnum.Day, Amount, null!)),
        (() => new DateAdd(DateAddDateTimePartEnum.Nanosecond, null!, Value)),
        (() => new DateAdd(DateAddDateTimePartEnum.Nanosecond, Amount, null!)),
    ];



    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new DateAdd(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn, ColumnA)),
        (() => new DateAdd(SharedDateTimePartEnum.Day, ColumnA, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new DateAdd(SharedDateTimePartEnum.Day, First, Second),
        new DateAdd(SharedDateTimePartEnum.Day, First, Third),
        new DateAdd(SharedDateTimePartEnum.Month, First, Second),
        new DateAdd(SharedDateTimePartEnum.Day, ColumnA, ParameterValue),
        new DateAdd(SharedDateTimePartEnum.Day, ColumnB, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new DateAdd(SharedDateTimePartEnum.Day, ColumnA, ParameterValue),
        new DateAdd(SharedDateTimePartEnum.Day, ColumnA, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new DateAdd(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn, Second),
        new DateAdd(SharedDateTimePartEnum.Day, First, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new DateAdd(SharedDateTimePartEnum.Day, First, Second),
        new DateAdd(SharedDateTimePartEnum.Day, ColumnA, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new DateAdd(SharedDateTimePartEnum.Day, ColumnA, Second),
        new DateAdd(SharedDateTimePartEnum.Day, First, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new DateAdd(SharedDateTimePartEnum.Day, First, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new DateAdd(SharedDateTimePartEnum.Day, First, Second),
        new DateAdd(SharedDateTimePartEnum.Day, ColumnA, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new DateAdd(SharedDateTimePartEnum.Day, ColumnA, ColumnB),
    ];

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("DATEADD(day, Amount, Value)", new DateAdd(SharedDateTimePartEnum.Day, Amount, DateValue).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("DATEADD(nanosecond, Amount, Value)", new DateAdd(DateAddDateTimePartEnum.Nanosecond, Amount, DateValue).ToString());
}
