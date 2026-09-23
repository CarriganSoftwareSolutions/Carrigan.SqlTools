using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class RepeatWithCountTests : FunctionTestsWithOneExpressionOneIntBase
{
    protected override string ExpectedFunctionName =>
        "REPEAT";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, int precision) =>
        new Repeat(sqlExpression!, precision);
}
