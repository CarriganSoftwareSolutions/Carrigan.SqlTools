using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;


public class RoundWithPrecisionTests : FunctionTestsWithOneExpressionOneIntBase
{
    protected override string ExpectedFunctionName => "ROUND";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, int precision) =>
        new Round(sqlExpression!, precision);
}