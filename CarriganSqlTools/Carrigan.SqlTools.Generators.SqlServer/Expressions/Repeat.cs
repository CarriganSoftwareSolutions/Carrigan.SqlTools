namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server <c>REPLICATE</c> function.
/// </summary>
public class Repeat : FunctionalExpression
{
    /// <summary>
    /// Gets the SQL Server function name.
    /// </summary>
    protected override string FunctionName =>
        "REPLICATE";

    /// <summary>
    /// Initializes a new instance of the <see cref="Repeat"/> class with a constant repeat count.
    /// </summary>
    /// <param name="expression">The string expression to repeat.</param>
    /// <param name="count">The number of repetitions. The value is bound through a SQL parameter.</param>
    public Repeat(SqlExpression expression, int count) : base([ValidateValue(expression), ValidateParameterValue(count)])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Repeat"/> class with a repeat count supplied by another SQL expression.
    /// </summary>
    /// <param name="expression">The string expression to repeat.</param>
    /// <param name="count">The SQL expression that supplies the number of repetitions.</param>
    public Repeat(SqlExpression expression, SqlExpression count) : base([ValidateValue(expression), ValidateValue(count)])
    {
    }
}
