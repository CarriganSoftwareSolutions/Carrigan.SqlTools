using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base class for SQL logical operators (e.g., <c>AND</c>, <c>OR</c>) used to combine
/// one or more predicate expressions in <c>WHERE</c> and <c>JOIN</c> clauses.
/// </summary>
public abstract class LogicalOperator : Predicates
{
    /// <summary>
    /// The SQL logical operator text placed between rendered child predicates.
    /// </summary>
    private readonly string _operator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogicalOperator"/> class for the specified
    /// SQL logical operator (e.g., <c>AND</c>, <c>OR</c>) and predicates.
    /// </summary>
    /// <remarks>
    /// Behavior:
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///     If no predicates are provided, an <see cref="ArgumentException"/> is thrown.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     If exactly one predicate is provided, that predicate is emitted directly without
    ///     adding the operator.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     If two or more predicates are provided, they are combined with the specified operator.
    ///     </description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <param name="op">The SQL operator token to use (e.g., <c>"AND"</c>, <c>"OR"</c>).</param>
    /// <param name="predicates">One or more boolean predicate expressions to combine.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="op"/> or <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="op"/> is empty or whitespace, or when <paramref name="predicates"/> contains no elements.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="predicates"/> contains disallowed <c>null</c> values.
    /// </exception>
    public LogicalOperator(string op, params IEnumerable<Predicates> predicates)
        : this(ValidateOperator(op), ValidatePredicates(predicates))
    {
    }

    private LogicalOperator(string op, IReadOnlyList<Predicates> predicates)
        : base(predicates, ToDialectNeutralString(op, predicates)) =>
        _operator = op;

    private static string ValidateOperator(string op)
    {
        ArgumentNullException.ThrowIfNull(op, nameof(op));

        if (op.IsNullOrWhiteSpace())
            throw new ArgumentException("Logical operator text cannot be empty or whitespace.", nameof(op));

        return op;
    }

    private static IReadOnlyList<Predicates> ValidatePredicates(IEnumerable<Predicates> predicates)
    {
        ArgumentNullException.ThrowIfNull(predicates, nameof(predicates));

        Predicates[] predicateArray = [.. predicates];
        if (predicateArray.Length == 0)
            throw new ArgumentException($"{nameof(predicates)} must contain at least one value.", nameof(predicates));
        if (predicateArray.Any(static predicate => predicate is null))
            throw new NullReferenceException($"{nameof(predicates)} cannot contain null values.");

        return predicateArray;
    }

    private static string ToDialectNeutralString(string op, IReadOnlyList<Predicates> predicates) =>
        predicates.Count == 1 ? predicates[0].ToString() : $"({string.Join($" {op} ", predicates)})";

    /// <summary>
    /// Generates the SQL fragment represented by this logical operator.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render each child predicate.</param>
    /// <returns>
    /// A SQL string for the combined predicates. If a single predicate was provided, returns that
    /// predicate’s SQL without adding the operator; otherwise returns the predicates joined by the
    /// operator and wrapped in parentheses.
    /// </returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        int index = 0;

        if (ChildNodes.Count() == 1)
        {
            foreach (ISqlFragment fragment in ChildNodes.Single().ToSqlFragments(dialect))
                yield return fragment;

            yield break;
        }

        yield return new SqlFragmentText("(");
        foreach (SqlExpression predicate in ChildNodes)
        {
            if (index > 0)
                yield return new SqlFragmentText($" {_operator} ");
            foreach (ISqlFragment fragment in predicate.ToSqlFragments(dialect))
                yield return fragment;
            index++;
        }
        yield return new SqlFragmentText(")");
    }
}
