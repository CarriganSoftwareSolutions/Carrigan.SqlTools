using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

/// <summary>
/// Tests for the GetDate function in SQL Server, which returns the current database system timestamp as a datetime value.
/// </summary>
public class GetDateTests : FunctionTestsNoArgumentBase
{
    /// <summary>
    /// Gets the expected function name for the GetDate function, which is "GETDATE".
    /// </summary>
    protected override string ExpectedFunctionName =>
        "GETDATE";

    /// <summary>
    /// Creates a new instance of the GetDate functional expression.
    /// </summary>
    /// <returns>
    /// A new instance of the GetDate functional expression.
    /// </returns>
    protected override FunctionalExpression New() =>
        new GetDate();
}
