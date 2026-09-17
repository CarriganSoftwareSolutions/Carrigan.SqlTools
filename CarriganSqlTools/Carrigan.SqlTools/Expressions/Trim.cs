namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL <c>TRIM</c> function, which removes spaces or specified characters from both ends of a string.
/// </summary>
/// <remarks>
/// <para>
/// The single-value form renders as <c>TRIM(value)</c>. The characters overloads currently render as
/// <c>TRIM(value, characters)</c>, with <c>characters</c> represented by a SQL parameter rather than embedded in the SQL text.
/// </para>
/// <para>
/// SQL Server supports the single-value <c>TRIM</c> form beginning with SQL Server 2017 (14.x), but SQL Server does not use the
/// comma-separated <c>TRIM(value, characters)</c> form for custom characters; it uses <c>TRIM(characters FROM value)</c>.
/// SQL Server 2022 (16.x) with compatibility level 160 adds <c>LEADING</c>, <c>TRAILING</c>, and <c>BOTH</c>, which this class
/// intentionally does not expose.
/// </para>
/// <para>
/// PostgreSQL supports the comma-separated two-value form as non-standard <c>TRIM</c> syntax. Beginning with PostgreSQL 8.3,
/// non-string values are no longer implicitly coerced to <c>text</c> and may require an explicit cast.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// Trim expression = new(new Column<Customer>(nameof(Customer.Name)));
/// Trim expressionWithCharacters = new(new Column<Customer>(nameof(Customer.Name)), " x");
/// ]]></code>
/// </example>
public class Trim : FunctionalExpression
{
    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "TRIM";

    /// <summary>
    /// Initializes a new instance of the <see cref="Trim"/> class that removes leading and trailing spaces.
    /// </summary>
    /// <param name="sqlExpression">The expression to trim.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpression"/> is <c>null</c>.
    /// </exception>
    public Trim(SqlExpression sqlExpression) : base([sqlExpression ?? throw new ArgumentNullException(nameof(sqlExpression))])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Trim"/> class that removes the specified characters from both ends.
    /// </summary>
    /// <param name="sqlExpression">The expression to trim.</param>
    /// <param name="characters">The characters to remove. The value is bound through a SQL parameter.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpression"/> or <paramref name="characters"/> is <c>null</c>.
    /// </exception>
    public Trim(SqlExpression sqlExpression, string characters) :
        base
        (
            [
                sqlExpression ?? throw new ArgumentNullException(nameof(sqlExpression)),
                new Parameter(characters ?? throw new ArgumentNullException(nameof(characters)))
            ]
        )
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Trim"/> class that removes the specified characters from both ends.
    /// </summary>
    /// <param name="sqlExpression">The expression to trim.</param>
    /// <param name="characters">
    /// The characters to remove. The array is materialized as a string and bound through a SQL parameter.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpression"/> or <paramref name="characters"/> is <c>null</c>.
    /// </exception>
    public Trim(SqlExpression sqlExpression, char[] characters) :
        this(sqlExpression, new string(characters ?? throw new ArgumentNullException(nameof(characters))))
    {
    }

    /// <summary>
    /// Determines aggregate status from the value being trimmed. The internally generated characters parameter does not
    /// change whether the expression is aggregate.
    /// </summary>
    public override bool IsAggregate() =>
        ChildNodes.First().IsAggregate();
}