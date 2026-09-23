using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class DateTruncTests
{
    private static Parameter Value => new(new DateTime(2026, 9, 23, 6, 30, 15), "Value");

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("DATE_TRUNC('day', Value)", new DateTrunc(SharedDateTimePartEnum.Day, Value).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("DATE_TRUNC('quarter', Value)", new DateTrunc(DateTruncDateTimePartEnum.Quarter, Value).ToString());
}
