using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class MakeIntervalTests
{
    private static Parameter Value => new(2, "Value");

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("MAKE_INTERVAL(days => Value)", new MakeInterval(SharedDateTimePartEnum.Day, Value).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("MAKE_INTERVAL(days => Value)", new MakeInterval(MakeIntervalDateTimePartEnum.Day, Value).ToString());
}
