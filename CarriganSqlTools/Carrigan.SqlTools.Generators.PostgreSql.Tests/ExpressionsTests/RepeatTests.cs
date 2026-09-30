using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class RepeatTests : FunctionTestsWithTwoExpressionsBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Repeat(null!, Second)),
        (() => new Repeat(First, null!)),
    ];


    protected override string ExpectedFunctionName =>
        "REPEAT";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second) =>
        new Repeat(first!, second!);
}
