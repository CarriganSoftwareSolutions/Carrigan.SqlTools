using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class LeftTests : FunctionTestsWithTwoExpressionsBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Left(null!, null!)),
        (() => new Left(First, null!)),
        (() => new Left(null!, Second)),
    ];

    protected override string ExpectedFunctionName =>
        "LEFT";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second) =>
        new Left(first!, second!);
}
