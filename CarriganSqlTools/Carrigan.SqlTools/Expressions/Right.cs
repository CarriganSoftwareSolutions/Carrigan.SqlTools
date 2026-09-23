namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL <c>RIGHT</c> function, which returns the requested number of characters from the end of a string expression.
/// </summary>
public class Right : FunctionalExpression
{
    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "RIGHT";

    /// <summary>
    /// Initializes a new instance of the <see cref="Right"/> class with a constant character count.
    /// </summary>
    /// <param name="expression">The string expression.</param>
    /// <param name="length">The number of characters to return. The value is bound through a SQL parameter.</param>
    public Right(SqlExpression expression, int length) : base([ValidateValue(expression), ValidateParameterValue(length)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Right"/> class with a character count supplied by another SQL expression.
    /// </summary>
    /// <param name="expression">The string expression.</param>
    /// <param name="length">The SQL expression that supplies the number of characters to return.</param>
    public Right(SqlExpression expression, SqlExpression length) : base([ValidateValue(expression), ValidateValue(length)])
    {
    }
}
