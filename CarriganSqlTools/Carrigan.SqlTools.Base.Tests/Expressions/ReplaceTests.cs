using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class ReplaceTests : FunctionTestsWithThreeExpressionsBase
{
    protected override string ExpectedFunctionName =>
        "REPLACE";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second, SqlExpression? third) =>
        new Replace(first!, second!, third!);
}
