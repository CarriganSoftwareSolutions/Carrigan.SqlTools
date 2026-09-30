using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class SquareRootTests : FunctionTestsWithSingleValueBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new SquareRoot(null!)),
    ];
    protected override string ExpectedFunctionName =>
        "SQRT";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) =>
        new SquareRoot(sqlExpression!);
}
