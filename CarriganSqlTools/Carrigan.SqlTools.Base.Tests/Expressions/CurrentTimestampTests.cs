using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class CurrentTimestampTests : FunctionTestsNoArgumentBase
{
    protected override string ExpectedFunctionName =>
        "CURRENT_TIMESTAMP";

    protected override FunctionalExpression New() =>
        new CurrentTimeStamp();
}
