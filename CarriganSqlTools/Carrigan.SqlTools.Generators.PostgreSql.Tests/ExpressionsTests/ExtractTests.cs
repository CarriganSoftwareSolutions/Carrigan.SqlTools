using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class ExtractTests
{
    private static Parameter Value => new(new DateTime(2026, 9, 23, 6, 30, 0), "Value");

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("EXTRACT(day FROM Value)", new Extract(SharedDateTimePartEnum.Day, Value).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("EXTRACT(epoch FROM Value)", new Extract(ExtractDateTimePartEnum.Epoch, Value).ToString());
}
