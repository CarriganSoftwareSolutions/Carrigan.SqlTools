using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class LeftTests : FunctionTestsWithTwoExpressionsBase
{
    protected override string ExpectedFunctionName =>
        "LEFT";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second) =>
        new Left(first!, second!);
}
