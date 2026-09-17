using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;


public class RTrimWithCharactersTests : FunctionTestsWithTwoValuesBase
{
    protected override string ExpectedFunctionName => "RTRIM";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, string? characters) =>
        new RTrim(sqlExpression!, characters!);

    protected override FunctionalExpression New(SqlExpression? sqlExpression, char[]? characters) =>
        new RTrim(sqlExpression!, characters!);
}
