using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class RepeatTests : FunctionTestsWithTwoExpressionsBase
{
    protected override string ExpectedFunctionName =>
        "REPEAT";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second) =>
        new Repeat(first!, second!);
}
