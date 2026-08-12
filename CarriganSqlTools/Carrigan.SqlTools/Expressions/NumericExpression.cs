

using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Expressions;

public abstract class NumericExpression : SqlExpression
{
    /// <summary>
    /// Base constructor for all numeric expression classes.
    /// </summary>
    /// <param name="childExpressions">Represents all child nodes for a given expression.</param>
    /// <param name="dialectNeutralStringRepresentation">
    /// Represents a dialect-neutral string representation of the expression, used for debugging, logging, and key-value pairs.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="childExpressions"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="childExpressions"/> contains disallowed <c>null</c> values.
    /// </exception>
    protected NumericExpression(IEnumerable<SqlExpression> childExpressions, string dialectNeutralStringRepresentation)
        : base(childExpressions, dialectNeutralStringRepresentation)
    {
    }

    /// <summary>
    /// Implicitly converts a <see cref="Parameter"/> to a <see cref="NumericExpression"/> by wrapping it in a <see cref="NumericParameter"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="Parameter"/> to convert to a <see cref="NumericExpression"/>.
    /// </param>
    //TODO: create Roslyn analyzer to warn against using this.
    //TODO: unite tests
    public static implicit operator NumericExpression(Parameter parameter) =>
        new NumericParameter(parameter);
}
