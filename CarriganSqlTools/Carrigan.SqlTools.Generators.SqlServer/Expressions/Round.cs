namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL <c>ROUND</c> function, which rounds a numeric expression to a specified precision.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Round expression = new(new Column<Customer>(nameof(Customer.Name)));
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
/// SELECT ROUND(\"Customer\".\"Name\") AS \"Value\" FROM \"Customer\"
/// ]]></code>
/// </example>
/// <example>
/// <code language="csharp"><![CDATA[
/// Round expression = new(new Column<Customer>(nameof(Customer.Name)), 2);
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
/// SELECT ROUND(\"Customer\".\"Name\", $1) AS \"Value\" FROM \"Customer\"
/// ]]></code>
/// </example>
public class Round : FunctionalExpression
{
    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "ROUND";

    /// <summary>
    /// Initializes a new instance of the <see cref="Round"/> class that rounds the specified SQL expression to the nearest integer.
    /// </summary>
    /// <param name="sqlExpression">
    /// The SQL expression to round.
    /// </param>
    public Round(SqlExpression sqlExpression) : this(sqlExpression, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Round"/> class that rounds the specified SQL expression to the specified number of decimal places.
    /// </summary>
    /// <param name="sqlExpression">
    /// The SQL expression to round.
    /// </param>
    /// <param name="precision">
    /// The number of decimal places to round to. A positive value rounds to the right of the decimal point, a negative value rounds to the left of the decimal
    /// point, and zero rounds to the nearest integer.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpression"/> is <c>null</c>.
    /// </exception>
    public Round(SqlExpression sqlExpression, int precision) :
        base
        (
            [
                sqlExpression ?? throw new ArgumentNullException(nameof(sqlExpression)),
                new Parameter(precision)
            ]
        )
    {
    }
}