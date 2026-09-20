using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class ReplaceWithTwoStringsTests : FunctionTestsWithOneExpressionTwoStringsBase
{
    protected override string ExpectedFunctionName =>
        "REPLACE";

    protected override FunctionalExpression New(SqlExpression? first, string? second, string? third) =>
        new Replace(first!, second!, third!);
}
