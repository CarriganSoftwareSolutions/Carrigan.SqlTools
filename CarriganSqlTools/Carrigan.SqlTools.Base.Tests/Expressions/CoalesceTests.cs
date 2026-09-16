using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class CoalesceTests : FunctionTestsWithMultipleValuesBase
{
    protected override string ExpectedFunctionName =>
        "COALESCE";

    protected override FunctionalExpression New(params IEnumerable<SqlExpression>? sqlExpression) =>
        new Coalesce(sqlExpression!);
}
