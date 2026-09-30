using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class UpperTests : FunctionTestsWithSingleValueBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Upper(null!)),
    ];

    protected override string ExpectedFunctionName => "UPPER";

    protected override FunctionalExpression New(SqlExpression? sqlExpression) => new Upper(sqlExpression!);
}
