using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class RTrimTests : FunctionTestsWithSingleValueBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new RTrim(null!)),
    ];
    protected override string ExpectedFunctionName => "RTRIM";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) =>
        new RTrim(sqlExpression!);
}