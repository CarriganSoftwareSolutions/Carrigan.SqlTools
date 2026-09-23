namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server <c>LEN</c> function.
/// </summary>
public class Length : FunctionalExpression
{
    /// <summary>
    /// Gets the SQL Server function name.
    /// </summary>
    protected override string FunctionName =>
        "LEN";

    /// <summary>
    /// Initializes a new instance of the <see cref="Length"/> class.
    /// </summary>
    /// <param name="expression">The string expression whose length is returned.</param>
    public Length(SqlExpression expression) : base([ValidateValue(expression)])
    {
    }
}
