using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class LeftWithLengthTests : FunctionTestsWithOneExpressionOneIntBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Left(null!, 2)),
    ];

    protected override string ExpectedFunctionName =>
        "LEFT";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, int precision) =>
        new Left(sqlExpression!, precision);
}
