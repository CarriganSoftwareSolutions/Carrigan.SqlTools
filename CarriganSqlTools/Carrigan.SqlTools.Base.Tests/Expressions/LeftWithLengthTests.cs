using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class LeftWithLengthTests : FunctionTestsWithOneExpressionOneIntBase
{
    protected override string ExpectedFunctionName =>
        "LEFT";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, int precision) =>
        new Left(sqlExpression!, precision);
}
