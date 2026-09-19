using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class RoundTests : FunctionTestsWithSingleValueBase
{
    protected override string ExpectedFunctionName => "ROUND";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) =>
        new Round(sqlExpression!);
}