using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server-compatible nested <c>LTRIM(RTRIM(value))</c> expression.
/// </summary>
/// <remarks>
/// This expression is useful when targeting SQL Server versions earlier than SQL Server 2017, where the single-value
/// <c>TRIM</c> function is unavailable. It removes ordinary leading and trailing spaces only; it does not expose custom
/// characters. Newer SQL Server versions can generally use <see cref="Trim"/> for the single-value case.
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// LTrim expression = new(new Column<Customer>(nameof(Customer.Name)));
/// SelectTags selects = new(new SelectTag(expression, "Value"));
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Selects = selects
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT LTRIM([Customer].[Name]) AS [Value] FROM [Customer]
/// ]]></code>
/// </example>
/// <example>
/// <code language="csharp"><![CDATA[
/// LTrim expression = new(new Column<Customer>(nameof(Customer.Name)), " x");
/// SelectTags selects = new(new SelectTag(expression, "Value"));
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Selects = selects
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT LTRIM([Customer].[Name], @Parameter_1) AS [Value] FROM [Customer]
/// ]]></code>
/// </example>
public class LRTrim : LTrim
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LRTrim"/> class.
    /// </summary>
    /// <param name="sqlExpression">The expression whose trailing spaces are removed before its leading spaces are removed.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpression"/> is <c>null</c>.
    /// </exception>
    public LRTrim(SqlExpression sqlExpression) :
        base(new RTrim(sqlExpression ?? throw new ArgumentNullException(nameof(sqlExpression))))
    {
    }
}