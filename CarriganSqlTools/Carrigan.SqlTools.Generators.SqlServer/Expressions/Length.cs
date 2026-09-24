using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server <c>LEN</c> function.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Selects = expression.AsSelectTag("Value")
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT LEN([Customer].[Name]) AS [Value] FROM [Customer]
/// ]]></code>
/// </example>
public class Length : FunctionalExpression
{
    /// <summary>
    /// Gets the SQL Server function name.
    /// </summary>
    protected override string FunctionName =>
        "LEN";

    /// <summary>
    /// Initializes a new instance of the <see cref="Length"/> class.
    /// </summary>
    /// <param name="expression">The string expression whose length is returned.</param>
    public Length(SqlExpression expression) : base([ValidateValue(expression)])
    {
    }
}
