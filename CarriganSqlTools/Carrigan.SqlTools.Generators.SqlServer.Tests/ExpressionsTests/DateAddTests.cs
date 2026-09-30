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


    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("DATEADD(day, Amount, Value)", new DateAdd(SharedDateTimePartEnum.Day, Amount, DateValue).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("DATEADD(nanosecond, Amount, Value)", new DateAdd(DateAddDateTimePartEnum.Nanosecond, Amount, DateValue).ToString());
}
