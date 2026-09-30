using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class EOMonthTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new EOMonth(null!)),
        (() => new EOMonth(null!, Second)),
        (() => new EOMonth(First, null!)),
    ];

    [Fact]
    public void SingleExpressionConstructor_Test() =>
        Assert.Equal("EOMONTH(Value)", new EOMonth(DateValue).ToString());

    [Fact]
    public void TwoExpressionConstructor_Test() =>
        Assert.Equal("EOMONTH(Value, Offset)", new EOMonth(DateValue, new Parameter(1, "Offset")).ToString());
}
