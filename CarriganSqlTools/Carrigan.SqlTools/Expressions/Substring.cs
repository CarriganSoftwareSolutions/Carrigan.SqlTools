namespace Carrigan.SqlTools.Expressions;
//ignore spelling: substring
/// <summary>
/// Represents the SQL <c>SUBSTRING</c> function, which returns a portion of a string expression.
/// </summary>
public class Substring : FunctionalExpression
{
    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "SUBSTRING";

    /// <summary>
    /// Initializes a new instance of the <see cref="Substring"/> class with constant start and length values.
    /// </summary>
    /// <param name="expression">The string expression.</param>
    /// <param name="start">The one-based starting position. The value is bound through a SQL parameter.</param>
    /// <param name="length">The number of characters to return. The value is bound through a SQL parameter.</param>
    public Substring(SqlExpression expression, int start, int length) :
        base([ValidateValue(expression), ValidateParameterValue(start), ValidateParameterValue(length)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Substring"/> class with start and length values supplied by SQL expressions.
    /// </summary>
    /// <param name="expression">The string expression.</param>
    /// <param name="start">The SQL expression that supplies the one-based starting position.</param>
    /// <param name="length">The SQL expression that supplies the number of characters to return.</param>
    public Substring(SqlExpression expression, SqlExpression start, SqlExpression length) :
        base([ValidateValue(expression), ValidateValue(start), ValidateValue(length)])
    {
    }
}
