using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;


public class RTrimWithCharactersTests : FunctionTestsWithOneExpressionOneStringBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new RTrim(null!, "Test")),
        (() => new RTrim(Value, (string)null!)),
        (() => new RTrim(null!, ['T', 'e', 's', 't'])),
        (() => new RTrim(Value, (char[])null!)),
    ];
    protected override string ExpectedFunctionName => "RTRIM";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, string? characters) =>
        new RTrim(sqlExpression!, characters!);

    protected override FunctionalExpression New(SqlExpression? sqlExpression, char[]? characters) =>
        new RTrim(sqlExpression!, characters!);
}
