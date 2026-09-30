using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class LengthTests : FunctionTestsWithSingleValueBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Length(null!)),
    ];

    protected override string ExpectedFunctionName =>
        "LENGTH";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) =>
        new Length(sqlExpression!);
}
