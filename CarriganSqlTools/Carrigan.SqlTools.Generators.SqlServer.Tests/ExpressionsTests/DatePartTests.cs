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
