using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class SubstringWithConstantsTests : FunctionTestsWithOneExpressionTwoIntsBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Substring(null!, 2, 3)),
    ];
    protected override string ExpectedFunctionName =>
        "SUBSTRING";

    protected override FunctionalExpression New(SqlExpression? expression, int first, int second) =>
        new Substring(expression!, first, second);
}
