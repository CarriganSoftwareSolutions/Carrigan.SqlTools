using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class EOMonthTests
{
    private static Parameter Value => new(new DateTime(2026, 9, 23), "Value");

    [Fact]
    public void SingleExpressionConstructor_Test() =>
        Assert.Equal("EOMONTH(Value)", new EOMonth(Value).ToString());

    [Fact]
    public void TwoExpressionConstructor_Test() =>
        Assert.Equal("EOMONTH(Value, Offset)", new EOMonth(Value, new Parameter(1, "Offset")).ToString());
}
