using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class TrimTests : FunctionTestsWithSingleValueBase
{
    protected override string ExpectedFunctionName => "TRIM";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) =>
        new Trim(sqlExpression!);
}