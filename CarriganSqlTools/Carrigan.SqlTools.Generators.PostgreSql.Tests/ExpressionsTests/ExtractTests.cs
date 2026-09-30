using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class ExtractTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Extract(SharedDateTimePartEnum.Day, null!)),
        (() => new Extract(ExtractDateTimePartEnum.Epoch, null!)),
    ];

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("EXTRACT(day FROM Value)", new Extract(SharedDateTimePartEnum.Day, DateValue).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("EXTRACT(epoch FROM Value)", new Extract(ExtractDateTimePartEnum.Epoch, Value).ToString());
}
