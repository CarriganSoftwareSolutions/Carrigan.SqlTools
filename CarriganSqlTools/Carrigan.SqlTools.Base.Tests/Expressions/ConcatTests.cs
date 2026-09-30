using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class ConcatTests : FunctionTestsWithMultipleValuesBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Concat(null!, Value)),
        (() => new Concat(Value, null!)),
        (() => new Concat(null!, Value, First)),
        (() => new Concat(Value, null!, First)),
        (() => new Concat(Value, First, null!)),
    ];
    protected override string ExpectedFunctionName =>
        "CONCAT";

    protected override FunctionalExpression New(params IEnumerable<SqlExpression>? sqlExpression) =>
        new Concat(sqlExpression!);
}
