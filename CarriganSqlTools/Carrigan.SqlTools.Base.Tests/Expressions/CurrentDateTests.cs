using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class CurrentDateTests : FunctionTestsNoArgumentBase
{
    protected override string ExpectedFunctionName =>
        "CURRENT_DATE";

    protected override FunctionalExpression New() =>
        new CurrentDate();
}
