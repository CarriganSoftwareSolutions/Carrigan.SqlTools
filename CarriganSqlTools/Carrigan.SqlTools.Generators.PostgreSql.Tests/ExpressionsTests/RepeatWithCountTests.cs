using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class RepeatWithCountTests : FunctionTestsWithOneExpressionOneIntBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Repeat(null!, 42)),
    ];


    protected override string ExpectedFunctionName =>
        "REPEAT";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, int precision) =>
        new Repeat(sqlExpression!, precision);
}
