using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class SubstringTests : FunctionTestsWithThreeExpressionsBase
{
    protected override string ExpectedFunctionName =>
        "SUBSTRING";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second, SqlExpression? third) =>
        new Substring(first!, second!, third!);
}
