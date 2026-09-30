using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

//IGNORE SPELLING: Trunc

public class DateTruncTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new DateTrunc(SharedDateTimePartEnum.Day, null!)),
        (() => new DateTrunc(DateTruncDateTimePartEnum.IsoWeek, null!)),
    ];

    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("DATETRUNC(day, Value)", new DateTrunc(SharedDateTimePartEnum.Day, DateValue).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("DATETRUNC(iso_week, Value)", new DateTrunc(DateTruncDateTimePartEnum.IsoWeek, DateValue).ToString());
}
