using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class LowerTests : FunctionTestsWithSingleValueBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Ceiling(null!)),
    ];

    protected override string ExpectedFunctionName => "LOWER";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) => new Lower(sqlExpression!);
}
