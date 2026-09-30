using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class RightTests : FunctionTestsWithTwoExpressionsBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Right(null!, null!)),
        (() => new Right(First, null!)),
        (() => new Right(null!, Second)),
    ];
    protected override string ExpectedFunctionName =>
        "RIGHT";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second) =>
        new Right(first!, second!);
}
