using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class RightTests : FunctionTestsWithTwoExpressionsBase
{
    protected override string ExpectedFunctionName =>
        "RIGHT";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second) =>
        new Right(first!, second!);
}
