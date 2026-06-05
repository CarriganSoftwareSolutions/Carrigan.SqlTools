using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base class for SQL comparison predicates (e.g., <c>=</c>, <c>&lt;&gt;</c>, <c>&gt;</c>, <c>&lt;</c>, etc.).
/// Combines two child <see cref="Predicates"/> nodes with a SQL comparison operator and
/// participates in recursive SQL/parameter generation.
/// </summary>
public abstract class ComparisonOperator : Predicates
{
    /// <summary>
    /// The left-side predicate of the comparison.
    /// </summary>
    private readonly Predicates _left;

    /// <summary>
    /// The right-side predicate of the comparison.
    /// </summary>
    private readonly Predicates _right;

    /// <summary>
    /// The SQL text for the comparison operator (e.g., <c>=</c>, <c>&lt;&gt;</c>, <c>&gt;</c>, <c>&lt;</c>).
    /// </summary>
    private readonly string _operator;

    /// <summary>
    /// Base constructor for comparison operators.
    /// </summary>
    /// <param name="left">The left-side predicate.</param>
    /// <param name="right">The right-side predicate.</param>
    /// <param name="op">The SQL representation of the comparison operator.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="left"/> or <paramref name="right"/> or <paramref name="op"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="op"/> is empty or whitespace.
    /// </exception>
    public ComparisonOperator(Predicates left, Predicates right, string op) : base([left, right])
    {
        ArgumentNullException.ThrowIfNull(left, nameof(left));
        ArgumentNullException.ThrowIfNull(right, nameof(right));
        ArgumentNullException.ThrowIfNull(op, nameof(op));

        if (string.IsNullOrWhiteSpace(op))
            throw new ArgumentException("Comparison operator text cannot be empty or whitespace.", nameof(op));

        _left = left;
        _right = right;
        _operator = op;
    }

    /// <summary>
    /// Produces the SQL fragment represented by this comparison operator and its operands.
    /// </summary>
    /// <param name="dialect">The SQL dialect for which to generate the fragment.</param>
    /// <returns>
    /// A SQL fragment in the form <c>(&lt;left-sql&gt; OP &lt;right-sql&gt;)</c>, e.g.,
    /// <c>([T].[Col] = @Parameter_Col)</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="dialect"/> is <c>null</c>.
    /// </exception>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        ArgumentNullException.ThrowIfNull(dialect);

        yield return new SqlFragmentText("(");

        foreach (ISqlFragment fragment in _left.ToSqlFragments(dialect))
            yield return fragment;

        yield return new SqlFragmentText($" {_operator} ");

        foreach (ISqlFragment fragment in _right.ToSqlFragments(dialect))
            yield return fragment;

        yield return new SqlFragmentText(")");
    }
}
