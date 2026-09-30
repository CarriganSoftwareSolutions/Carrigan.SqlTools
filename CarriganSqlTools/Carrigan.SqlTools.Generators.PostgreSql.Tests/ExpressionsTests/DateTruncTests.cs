using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

//IGNORE SPELLING: Trunc

public class DateTruncTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new DateTrunc(SharedDateTimePartEnum.Day, null!)),
        (() => new DateTrunc(DateTruncDateTimePartEnum.Quarter, null!)),
    ];



    [Fact]
    public void SharedEnumConstructor_Test() =>
        Assert.Equal("DATE_TRUNC('day', Value)", new DateTrunc(SharedDateTimePartEnum.Day, DateValue).ToString());

    [Fact]
    public void FunctionEnumConstructor_Test() =>
        Assert.Equal("DATE_TRUNC('quarter', Value)", new DateTrunc(DateTruncDateTimePartEnum.Quarter, Value).ToString());
}
