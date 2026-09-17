using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class RTrimTests : FunctionTestsWithSingleValueBase
{
    protected override string ExpectedFunctionName => "RTRIM";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) =>
        new RTrim(sqlExpression!);
}