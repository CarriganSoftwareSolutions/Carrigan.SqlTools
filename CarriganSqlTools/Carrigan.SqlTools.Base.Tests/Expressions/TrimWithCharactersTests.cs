using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class TrimWithCharactersTests : FunctionTestsWithOneExpressionOneStringBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Trim(null!, "Test")),
        (() => new Trim(Value, (string)null!)),
        (() => new Trim(null!, ['T', 'e', 's', 't'])),
        (() => new Trim(Value, (char[])null!)),
    ];
    protected override string ExpectedFunctionName => "TRIM";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, string? characters) =>
        new Trim(sqlExpression!, characters!);

    protected override FunctionalExpression New(SqlExpression? sqlExpression, char[]? characters) =>
        new Trim(sqlExpression!, characters!);
}