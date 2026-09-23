using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class SubstringWithConstantsTests : FunctionTestsWithOneExpressionTwoIntsBase
{
    protected override string ExpectedFunctionName =>
        "SUBSTRING";

    protected override FunctionalExpression New(SqlExpression? expression, int first, int second) =>
        new Substring(expression!, first, second);
}
