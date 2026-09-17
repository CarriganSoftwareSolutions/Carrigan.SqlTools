using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class TrimWithCharactersTests : FunctionTestsWithTwoValuesBase
{
    protected override string ExpectedFunctionName => "TRIM";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, string? characters) =>
        new Trim(sqlExpression!, characters!);

    protected override FunctionalExpression New(SqlExpression? sqlExpression, char[]? characters) =>
        new Trim(sqlExpression!, characters!);
}