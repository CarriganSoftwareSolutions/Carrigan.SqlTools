using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;


public class RoundWithPrecisionTests : FunctionTestsWithOneExpressionOneIntBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Round(null!, 42)),
    ];


    protected override string ExpectedFunctionName => "ROUND";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, int precision) =>
        new Round(sqlExpression!, precision);
}