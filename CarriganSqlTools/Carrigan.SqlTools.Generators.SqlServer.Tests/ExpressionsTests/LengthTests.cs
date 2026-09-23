using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class LengthTests : FunctionTestsWithSingleValueBase
{
    protected override string ExpectedFunctionName =>
        "LEN";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) =>
        new Length(sqlExpression!);
}
