using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class PowerWithNumericTests : FunctionTestsWithOneExpressionOneNumericBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Power(null!, 2)),
        (() => new Power(null!, 2f)),
        (() => new Power(null!, 2d)),
        (() => new Power(null!, 2m)),
    ];
    protected override string ExpectedFunctionName =>
        "POWER";

    protected override FunctionalExpression New(SqlExpression? sqlExpression, int number) =>
        new Power(sqlExpression!, number);
    protected override FunctionalExpression New(SqlExpression? sqlExpression, float number) =>
        new Power(sqlExpression!, number);
    protected override FunctionalExpression New(SqlExpression? sqlExpression, double number) =>
        new Power(sqlExpression!, number);
    protected override FunctionalExpression New(SqlExpression? sqlExpression, decimal number) =>
        new Power(sqlExpression!, number);
}
