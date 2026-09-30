using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class LTrimTests : FunctionTestsWithSingleValueBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new LTrim(null!)),
    ];

    protected override string ExpectedFunctionName => "LTRIM";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) =>
        new LTrim(sqlExpression!);
}