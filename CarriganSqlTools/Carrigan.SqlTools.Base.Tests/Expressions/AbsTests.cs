using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class AbsTests : FunctionTestsWithSingleValueBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Abs(null!))
    ];

    protected override FunctionalExpression New(SqlExpression? sqlExpression) => 
        new Abs(sqlExpression!);
    protected override string ExpectedFunctionName => "ABS";
}
