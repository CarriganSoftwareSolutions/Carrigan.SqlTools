using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class MakeIntervalTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new MakeInterval(SharedDateTimePartEnum.Day, null!)),
        (() => new MakeInterval(MakeIntervalDateTimePartEnum.Day, null!)),
    ];

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("MAKE_INTERVAL(days => Second)", new MakeInterval(SharedDateTimePartEnum.Day, Second).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("MAKE_INTERVAL(days => Second)", new MakeInterval(MakeIntervalDateTimePartEnum.Day, Second).ToString());
}
