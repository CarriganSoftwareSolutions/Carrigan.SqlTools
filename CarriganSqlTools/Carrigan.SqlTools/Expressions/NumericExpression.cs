


namespace Carrigan.SqlTools.Expressions;

[Obsolete("NumericExpression is obsolete. Use SqlExpression instead.")]
public abstract class NumericExpression : SqlExpression
{
    /// <summary>
    /// Base constructor for all numeric expression classes.
    /// </summary>
    /// <param name="childExpressions">Represents all child nodes for a given expression.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="childExpressions"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="childExpressions"/> contains disallowed <c>null</c> values.
    /// </exception>
    protected NumericExpression(IEnumerable<SqlExpression> childExpressions)
        : base(childExpressions)
    {
    }

    /// <summary>
    /// Implicitly converts a <see cref="Parameter"/> to a <see cref="NumericExpression"/> by wrapping it in a <see cref="NumericParameter"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="Parameter"/> to convert to a <see cref="NumericExpression"/>.
    /// </param>
    [Obsolete("NumericExpression is obsolete. Use SqlExpression and Parameter directly instead.")]
    public static implicit operator NumericExpression(Parameter parameter) =>
        new NumericParameter(parameter);
}
