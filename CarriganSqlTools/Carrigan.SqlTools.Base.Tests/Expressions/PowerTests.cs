using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class PowerTests : FunctionTestsWithTwoExpressionsBase
{
    protected override string ExpectedFunctionName =>
        "POWER";

    protected override FunctionalExpression New(SqlExpression? sqlExpression1, SqlExpression? sqlExpression2) =>
        new Power(sqlExpression1!, sqlExpression2!);
}
