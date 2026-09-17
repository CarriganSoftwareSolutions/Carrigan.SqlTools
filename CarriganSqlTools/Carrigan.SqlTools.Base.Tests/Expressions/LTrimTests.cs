using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class LTrimTests : FunctionTestsWithSingleValueBase
{
    protected override string ExpectedFunctionName => "LTRIM";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) =>
        new LTrim(sqlExpression!);
}