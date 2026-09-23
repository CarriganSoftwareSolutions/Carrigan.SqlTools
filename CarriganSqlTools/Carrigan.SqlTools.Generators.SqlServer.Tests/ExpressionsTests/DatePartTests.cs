using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class DatePartTests
{
    private static Parameter Value => new(new DateTime(2026, 9, 23, 6, 30, 0), "Value");

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("DATEPART(day, Value)", new DatePart(SharedDateTimePartEnum.Day, Value).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("DATEPART(iso_week, Value)", new DatePart(DatePartDateTimePartEnum.IsoWeek, Value).ToString());

    [Fact]
    public void DifferentParts_AreNotEqual() =>
        Assert.NotEqual(new DatePart(DatePartDateTimePartEnum.Year, Value), new DatePart(DatePartDateTimePartEnum.Month, Value));
}
