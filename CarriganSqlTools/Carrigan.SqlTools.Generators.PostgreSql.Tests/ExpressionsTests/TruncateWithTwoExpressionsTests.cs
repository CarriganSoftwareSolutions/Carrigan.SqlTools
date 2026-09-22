using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class TruncateWithTwoExpressionsTests : FunctionTestsWithTwoExpressionsBase
{
    protected override string ExpectedFunctionName => "TRUNC";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second) => new Truncate(first!, second!);
}
