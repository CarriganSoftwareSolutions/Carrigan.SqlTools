using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class DateAddTests
{
    private static Parameter Amount => new(2, "Amount");
    private static Parameter Value => new(new DateTime(2026, 9, 23), "Value");

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("DATEADD(day, Amount, Value)", new DateAdd(SharedDateTimePartEnum.Day, Amount, Value).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("DATEADD(nanosecond, Amount, Value)", new DateAdd(DateAddDateTimePartEnum.Nanosecond, Amount, Value).ToString());
}
