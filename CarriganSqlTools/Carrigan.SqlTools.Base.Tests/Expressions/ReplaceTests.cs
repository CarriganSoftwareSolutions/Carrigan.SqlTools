using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class ReplaceTests : FunctionTestsWithThreeExpressionsBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Replace(null!, Second, Third)),
        (() => new Replace(First, null!, Third)),
        (() => new Replace(First, Second, null!)),
    ];
    protected override string ExpectedFunctionName =>
        "REPLACE";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second, SqlExpression? third) =>
        new Replace(first!, second!, third!);
}
