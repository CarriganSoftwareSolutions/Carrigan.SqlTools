

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL REPLACE function expression.
/// </summary>
public class Replace : FunctionalExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Replace"/> class with the specified value, search string, and replacement string.
    /// </summary>
    /// <param name="value">
    /// The SQL expression representing the value to be modified.
    /// </param>
    /// <param name="search">
    /// The SQL expression representing the string to search for.
    /// </param>
    /// <param name="replacement">
    /// The SQL expression representing the string to replace the search string with.
    /// </param>
    public Replace(SqlExpression value, SqlExpression search, SqlExpression replacement) :
        base(ValidateValue(value), ValidateValue(search), ValidateValue(replacement))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Replace"/> class with the specified value, search string, and replacement string.
    /// </summary>
    /// <param name="value">
    /// The SQL expression representing the value to be modified.
    /// </param>
    /// <param name="search">
    /// The string to search for.
    /// </param>
    /// <param name="replacement">
    /// The string to replace the search string with.
    /// </param>
    public Replace(SqlExpression value, string search, string replacement) :
        base(ValidateValue(value), ValidateParameterValue(search), ValidateParameterValue(replacement))
    {
    }

    /// <summary>
    /// Gets the name of the SQL function represented by this expression.
    /// </summary>
    protected override string FunctionName =>
        "REPLACE";
}
