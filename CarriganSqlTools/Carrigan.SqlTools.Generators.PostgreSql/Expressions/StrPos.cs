namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the PostgreSQL <c>STRPOS</c> function.
/// </summary>
public class StrPos : FunctionalExpression
{
    /// <summary>
    /// Gets the PostgreSQL function name.
    /// </summary>
    protected override string FunctionName =>
        "STRPOS";

    /// <summary>
    /// Initializes a new instance of the <see cref="StrPos"/> class with a constant search string.
    /// </summary>
    /// <param name="expression">The string expression to search.</param>
    /// <param name="find">The string to find. The value is bound through a SQL parameter.</param>
    public StrPos(SqlExpression expression, string find) : base([ValidateValue(expression), ValidateParameterValue(find)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrPos"/> class with a search value supplied by another SQL expression.
    /// </summary>
    /// <param name="expression">The string expression to search.</param>
    /// <param name="find">The SQL expression that supplies the string to find.</param>
    public StrPos(SqlExpression expression, SqlExpression find) : base([ValidateValue(expression), ValidateValue(find)])
    {
    }
}
