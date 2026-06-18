namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Base class for SQL expressions whose result is boolean-valued.
/// </summary>
public abstract class BooleanExpression : SqlExpression
{
    /// <summary>
    /// Initializes a new <see cref="BooleanExpression"/> instance.
    /// </summary>
    /// <param name="childExpressions">The child boolean expressions for this node.</param>
    protected BooleanExpression(IEnumerable<BooleanExpression> childExpressions) : base(childExpressions)
    {
    }
}
