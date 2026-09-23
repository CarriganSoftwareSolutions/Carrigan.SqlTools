using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class IndexOfTests : FunctionTestsWithTwoExpressionsBase
{
    protected override string ExpectedFunctionName =>
        "STRPOS";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second) =>
        new IndexOf(first!, second!);
}
