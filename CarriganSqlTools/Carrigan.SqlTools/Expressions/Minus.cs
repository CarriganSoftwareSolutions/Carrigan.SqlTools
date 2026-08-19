using System.Collections.Generic;
namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL logical <c>-</c> arithmetic operator.
/// </summary>
public class Minus : Subtract
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Minus"/> class, representing
    /// </summary>
    /// <param name="numericExpressions">
    /// One or more numeric expressions to subtract using <c>-</c>.
    /// </param>
    public Minus(params IEnumerable<NumericExpression> numericExpressions) : base(numericExpressions)
    {
    }
}
