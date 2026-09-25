using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Linq.Expressions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL Server <c>STUFF</c> function, which deletes a specified number of characters from a string and
/// inserts a replacement expression at the same position.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Stuff expression = new(new Column<Customer>(nameof(Customer.Name)), 2, 3, "X");
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Selects = expression.AsSelectTag("Value")
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT STUFF([Customer].[Name], @Parameter_1, @Parameter_2, @Parameter_3) AS [Value] FROM [Customer]
/// ]]></code>
/// </example>
public class Stuff : FunctionalExpression
{
    /// <summary>
    /// Gets the SQL Server function name.
    /// </summary>
    protected override string FunctionName =>
        "STUFF";

    /// <summary>
    /// Initializes a <c>STUFF(expression, start, length, replacement)</c> expression using SQL expressions for all values.
    /// </summary>
    /// <param name="expression">The character expression to modify.</param>
    /// <param name="start">The one-based starting position.</param>
    /// <param name="length">The number of characters to remove.</param>
    /// <param name="replacement">The replacement expression to insert.</param>
    public Stuff(SqlExpression expression, SqlExpression start, SqlExpression length, SqlExpression replacement) :
        base([ValidateValue(expression), ValidateValue(start), ValidateValue(length), ValidateValue(replacement)])
    {
    }

    /// <summary>
    /// Initializes a <c>STUFF(expression, start, length, replacement)</c> expression using constant start, length, and
    /// replacement values. The constants are bound through SQL parameters.
    /// </summary>
    /// <param name="expression">The character expression to modify.</param>
    /// <param name="start">The one-based starting position.</param>
    /// <param name="length">The number of characters to remove.</param>
    /// <param name="replacement">The replacement string to insert.</param>
    public Stuff(SqlExpression expression, int start, int length, string replacement) :
        base
        ([
            ValidateValue(expression),
            ValidateParameterValue(start),
            ValidateParameterValue(length),
            ValidateParameterValue(replacement)
        ])
    {
    }
}
