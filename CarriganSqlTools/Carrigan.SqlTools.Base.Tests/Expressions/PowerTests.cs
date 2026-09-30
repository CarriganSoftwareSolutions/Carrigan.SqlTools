using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class PowerTests : FunctionTestsWithTwoExpressionsBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Power(null!, null!)),
        (() => new Power(First, null!)),
        (() => new Power(null!, Second)),
    ];

    protected override string ExpectedFunctionName =>
        "POWER";

    protected override FunctionalExpression New(SqlExpression? sqlExpression1, SqlExpression? sqlExpression2) =>
        new Power(sqlExpression1!, sqlExpression2!);
}
