using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class TrimTests : FunctionTestsWithSingleValueBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Trim(null!)),
    ];

    protected override string ExpectedFunctionName => "TRIM";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) =>
        new Trim(sqlExpression!);
}