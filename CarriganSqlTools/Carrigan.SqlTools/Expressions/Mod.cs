namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the SQL logical <c>%</c> arithmetic operator.
/// </summary>
public class Mod : Modulo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Mod"/> class, representing
    /// </summary>
    /// <param name="numericExpressions">
    /// One or more numeric expressions to  using <c>%</c>.
    /// </param>
    public Mod(params IEnumerable<NumericExpression> numericExpressions) : base(numericExpressions)
    {
    }
}
