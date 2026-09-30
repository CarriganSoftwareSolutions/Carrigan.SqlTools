using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class CoalesceTests : FunctionTestsWithMultipleValuesBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Coalesce(null!, Value)),
        (() => new Coalesce(Value, null!)),
        (() => new Coalesce(null!, Value, First)),
        (() => new Coalesce(Value, null!, First)),
        (() => new Coalesce(Value, First, null!)),
    ];
    protected override string ExpectedFunctionName =>
        "COALESCE";

    protected override FunctionalExpression New(params IEnumerable<SqlExpression>? sqlExpression) =>
        new Coalesce(sqlExpression!);
}
