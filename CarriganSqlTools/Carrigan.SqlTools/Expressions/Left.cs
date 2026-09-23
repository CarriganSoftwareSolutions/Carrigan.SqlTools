namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL <c>LEFT</c> function, which returns the requested number of characters from the beginning of a string expression.
/// </summary>
public class Left : FunctionalExpression
{
    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "LEFT";

    /// <summary>
    /// Initializes a new instance of the <see cref="Left"/> class with a constant character count.
    /// </summary>
    /// <param name="expression">The string expression.</param>
    /// <param name="length">The number of characters to return. The value is bound through a SQL parameter.</param>
    public Left(SqlExpression expression, int length) :  base([ValidateValue(expression), ValidateParameterValue(length)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Left"/> class with a character count supplied by another SQL expression.
    /// </summary>
    /// <param name="expression">The string expression.</param>
    /// <param name="length">The SQL expression that supplies the number of characters to return.</param>
    public Left(SqlExpression expression, SqlExpression length) :  base([ValidateValue(expression), ValidateValue(length)])
    {
    }
}
