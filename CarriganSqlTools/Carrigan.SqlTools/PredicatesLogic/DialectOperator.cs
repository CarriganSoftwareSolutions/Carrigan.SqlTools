using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base class for SQL Dialect predicates (e.g., <c>=</c>, <c>&lt;&gt;</c>, <c>&gt;</c>, <c>&lt;</c>, etc.).
/// Combines two child <see cref="Predicates"/> nodes with a SQL Dialect operator and
/// participates in recursive SQL/parameter generation.
/// </summary>
public abstract class DialectOperator : Predicates
{
    /// <summary>
    /// The left-side predicate of the Dialect operator.
    /// </summary>
    protected readonly SqlExpression _left;

    /// <summary>
    /// The right-side predicate of the Dialect operator.
    /// </summary>
    protected readonly SqlExpression _right;

    /// <summary>
    /// Base constructor for Dialect operators.
    /// </summary>
    /// <param name="left">The left-side predicate.</param>
    /// <param name="right">The right-side predicate.</param>
    /// <param name="dialectNeutralOperatorString">
    /// The dialect-neutral string representation of the operator (e.g., <c>=</c>, <c>&lt;&gt;</c>, <c>&gt;</c>, <c>&lt;</c>, etc.).
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="left"/> or <paramref name="right"/> is <c>null</c>.
    /// </exception>
    public DialectOperator(SqlExpression left, SqlExpression right, string dialectNeutralOperatorString)
        : base([left, right], ValidateDialectNeutralStringRepresentation(dialectNeutralOperatorString))
    {
        ArgumentNullException.ThrowIfNull(left, nameof(left));
        ArgumentNullException.ThrowIfNull(right, nameof(right));

        _left = left;
        _right = right;
    }

    /// <summary>
    /// Validates the dialect-neutral string representation used by the expression wrapper.
    /// </summary>
    private static string ValidateDialectNeutralStringRepresentation(string dialectNeutralOperatorString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dialectNeutralOperatorString, nameof(dialectNeutralOperatorString));
        return dialectNeutralOperatorString;
    }
}
