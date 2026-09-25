using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server <c>PATINDEX</c> function, which returns the one-based starting position of the first
/// occurrence of a wildcard pattern within a character expression, or zero when the pattern is not found.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// PatIndex expression = new("%[0-9]%", new Column<Customer>(nameof(Customer.Name)));
/// SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT PATINDEX(@Parameter_1, [Customer].[Name]) AS [Value] FROM [Customer]
/// ]]></code>
/// </example>
public class PatIndex : FunctionalExpression
{
    /// <summary>
    /// Gets the SQL Server function name.
    /// </summary>
    protected override string FunctionName =>
        "PATINDEX";

    /// <summary>
    /// Initializes a <c>PATINDEX(pattern, expression)</c> expression with a constant wildcard pattern.
    /// </summary>
    /// <param name="pattern">The SQL Server wildcard pattern. The value is bound through a SQL parameter.</param>
    /// <param name="expression">The character expression to search.</param>
    public PatIndex(string pattern, SqlExpression expression) :
        base([ValidateParameterValue(pattern), ValidateValue(expression)])
    {
    }

    /// <summary>
    /// Initializes a <c>PATINDEX(pattern, expression)</c> expression with a pattern supplied by another SQL expression.
    /// </summary>
    /// <param name="pattern">The SQL expression supplying the wildcard pattern.</param>
    /// <param name="expression">The character expression to search.</param>
    public PatIndex(SqlExpression pattern, SqlExpression expression) :
        base([ValidateValue(pattern), ValidateValue(expression)])
    {
    }
}
