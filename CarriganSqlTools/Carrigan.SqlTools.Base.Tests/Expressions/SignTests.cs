using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class SignTests : FunctionTestsWithSingleValueBase
{
    protected override string ExpectedFunctionName =>
        "SIGN";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) =>
        new Sign(sqlExpression!);
}
