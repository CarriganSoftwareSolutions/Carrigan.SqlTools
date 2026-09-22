using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class TruncateTests : FunctionTestsWithOneExpressionOneDefaultIntBase
{
    protected override string ExpectedFunctionName => "TRUNC";

    protected override int DefaultInt => 0;

    protected override FunctionalExpression New(SqlExpression? sqlExpression) => new Truncate(sqlExpression!);
}
