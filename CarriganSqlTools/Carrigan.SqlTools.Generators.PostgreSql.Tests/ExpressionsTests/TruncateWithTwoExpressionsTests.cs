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
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Truncate(null!, Second)),
        (() => new Truncate(First, null!)),
    ];


    protected override string ExpectedFunctionName => "TRUNC";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second) => new Truncate(first!, second!);
}
