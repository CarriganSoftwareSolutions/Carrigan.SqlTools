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
/// LRTrim expression = new(new Column<Customer>(nameof(Customer.Name)));
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