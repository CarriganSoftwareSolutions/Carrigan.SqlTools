using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class DatePartTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new DatePart(SharedDateTimePartEnum.Day, null!)),
        (() => new DatePart(DatePartDateTimePartEnum.IsoWeek, null!)),
    ];


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new DatePart(SharedDateTimePartEnum.Day, ParameterValue),
        new DatePart(SharedDateTimePartEnum.Day, DifferentParameter),
        new DatePart(SharedDateTimePartEnum.Day, ColumnA),
        new DatePart(SharedDateTimePartEnum.Day, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new DatePart(SharedDateTimePartEnum.Day, new Add(ColumnA, ParameterValue)),
        new DatePart(SharedDateTimePartEnum.Day, new Add(ColumnA, ParameterDifferentValue)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new DatePart(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new DatePart(SharedDateTimePartEnum.Day, ParameterValue),
        new DatePart(SharedDateTimePartEnum.Day, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new DatePart(SharedDateTimePartEnum.Day, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new DatePart(SharedDateTimePartEnum.Day, ParameterValue),
        new DatePart(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new DatePart(SharedDateTimePartEnum.Day, ParameterValue),
        new DatePart(SharedDateTimePartEnum.Day, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new DatePart(SharedDateTimePartEnum.Day, ColumnA),
    ];

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("DATEPART(day, Value)", new DatePart(SharedDateTimePartEnum.Day, DateValue).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("DATEPART(iso_week, Value)", new DatePart(DatePartDateTimePartEnum.IsoWeek, DateValue).ToString());

    [Fact]
    public void DifferentParts_AreNotEqual() =>
        Assert.NotEqual(new DatePart(DatePartDateTimePartEnum.Year, DateValue), new DatePart(DatePartDateTimePartEnum.Month, DateValue));
}
