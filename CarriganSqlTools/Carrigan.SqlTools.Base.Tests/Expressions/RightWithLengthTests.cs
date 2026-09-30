using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class RightWithLengthTests : FunctionTestsWithOneExpressionOneIntBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Right(null!, 2)),
    ];
    protected override string ExpectedFunctionName =>
        "RIGHT";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, int precision) =>
        new Right(sqlExpression!, precision);
}
