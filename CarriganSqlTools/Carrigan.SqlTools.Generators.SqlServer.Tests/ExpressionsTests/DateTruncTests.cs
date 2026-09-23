using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class DateTruncTests
{
    private static Parameter Value => new(new DateTime(2026, 9, 23, 6, 30, 15), "Value");

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("DATETRUNC(day, Value)", new DateTrunc(SharedDateTimePartEnum.Day, Value).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("DATETRUNC(iso_week, Value)", new DateTrunc(DateTruncDateTimePartEnum.IsoWeek, Value).ToString());
}
