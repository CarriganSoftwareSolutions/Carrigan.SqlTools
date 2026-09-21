using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class FloorTests : FunctionTestsWithSingleValueBase
{
    protected override string ExpectedFunctionName =>
        "FLOOR";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) =>
        new Floor(sqlExpression!);
}
