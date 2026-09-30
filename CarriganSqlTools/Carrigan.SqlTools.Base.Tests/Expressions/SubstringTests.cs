using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class SubstringTests : FunctionTestsWithThreeExpressionsBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Substring(null!, null!, null!)),
        (() => new Substring(null!, Second, Third)),
        (() => new Substring(First, null!, Third)),
        (() => new Substring(First, Second, null!)),
    ];
    protected override string ExpectedFunctionName =>
        "SUBSTRING";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second, SqlExpression? third) =>
        new Substring(first!, second!, third!);
}
