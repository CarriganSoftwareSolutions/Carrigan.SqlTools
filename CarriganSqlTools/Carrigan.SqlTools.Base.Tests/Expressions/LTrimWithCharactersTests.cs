using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;


public class LTrimWithCharactersTests : FunctionTestsWithOneExpressionOneStringBase
{
    protected override string ExpectedFunctionName => "LTRIM";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, string? characters) =>
        new LTrim(sqlExpression!, characters!);

    protected override FunctionalExpression New(SqlExpression? sqlExpression, char[]? characters) =>
        new LTrim(sqlExpression!, characters!);
}