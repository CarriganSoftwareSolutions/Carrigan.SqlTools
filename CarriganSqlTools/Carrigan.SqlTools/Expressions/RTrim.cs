namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL <c>RTRIM</c> function, which removes spaces or specified characters from the end of a string.
/// </summary>
/// <remarks>
/// <para>
/// The single-value form renders as <c>RTRIM(value)</c>. The characters overloads render as
/// <c>RTRIM(value, characters)</c>, with <c>characters</c> represented by a SQL parameter rather than embedded in the SQL text.
/// </para>
/// <para>
/// SQL Server supports the optional characters argument only on SQL Server 2022 (16.x) and later when the database
/// compatibility level is 160 or higher. PostgreSQL supports the optional characters argument; beginning with PostgreSQL 8.3,
/// non-string values are no longer implicitly coerced to <c>text</c> and may require an explicit cast.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// RTrim expression = new(new Column<Customer>(nameof(Customer.Name)));
/// RTrim expressionWithCharacters = new(new Column<Customer>(nameof(Customer.Name)), " x");
/// ]]></code>
/// </example>
public class RTrim : FunctionalExpression
{
    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "RTRIM";

    /// <summary>
    /// Initializes a new instance of the <see cref="RTrim"/> class that removes trailing spaces.
    /// </summary>
    /// <param name="sqlExpression">The expression to trim.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpression"/> is <c>null</c>.
    /// </exception>
    public RTrim(SqlExpression sqlExpression) : base([sqlExpression ?? throw new ArgumentNullException(nameof(sqlExpression))])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RTrim"/> class that removes the specified trailing characters.
    /// </summary>
    /// <param name="sqlExpression">The expression to trim.</param>
    /// <param name="characters">The characters to remove. The value is bound through a SQL parameter.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpression"/> or <paramref name="characters"/> is <c>null</c>.
    /// </exception>
    public RTrim(SqlExpression sqlExpression, string characters) :
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
    /// Initializes a new instance of the <see cref="RTrim"/> class that removes the specified trailing characters.
    /// </summary>
    /// <param name="sqlExpression">The expression to trim.</param>
    /// <param name="characters">
    /// The characters to remove. The array is materialized as a string and bound through a SQL parameter.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpression"/> or <paramref name="characters"/> is <c>null</c>.
    /// </exception>
    public RTrim(SqlExpression sqlExpression, char[] characters) :
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