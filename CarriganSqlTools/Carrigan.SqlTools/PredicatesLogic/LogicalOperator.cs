using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base class for SQL logical operators (e.g., <c>AND</c>, <c>OR</c>) used to combine
/// one or more predicate expressions in <c>WHERE</c> and <c>JOIN</c> clauses.
/// </summary>
public abstract class LogicalOperator : Predicates
{
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
    ///     If no predicates are provided, an <see cref="ArgumentNullException"/> is thrown.
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
    /// Thrown when <paramref name="op"/> is <c>null</c>, or when <paramref name="predicates"/> is <c>null</c>
    /// or contains no elements.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="op"/> is empty or whitespace.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="predicates"/> contains disallowed <c>null</c> values.
    /// </exception>
    public LogicalOperator(string op, params IEnumerable<Predicates> predicates) : base(predicates)
    {
        ArgumentNullException.ThrowIfNull(predicates, nameof(predicates));
        ArgumentNullException.ThrowIfNull(op, nameof(op));

        if (predicates.IsNullOrEmpty())
            throw new ArgumentException($"{nameof(predicates)} must contain at least one value.", nameof(predicates));
        if (op.IsNullOrWhiteSpace())
            throw new ArgumentException("Logical operator text cannot be empty or whitespace.", nameof(op));

        _operator = op;
    }

    /// <summary>
    /// Generates the SQL fragment represented by this logical operator.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated while traversing the predicate tree. This is appended to parameter
    /// names to keep them unique when duplicates occur.
    /// </param>
    /// <param name="branchName">
    /// The branch prefix that is prepended to the beginning of all parameter names in this predicate tree.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-specified parameter tags that are duplicates, used to decide when a prefix
    /// must be applied.
    /// </param>
    /// <returns>
    /// A SQL string for the combined predicates. If a single predicate was provided, returns that
    /// predicate’s SQL without adding the operator; otherwise returns the predicates joined by the
    /// operator and wrapped in parentheses.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="prefix"/> or <paramref name="branchName"/> or <paramref name="duplicates"/> is <c>null</c>.
    /// </exception>
    internal override IEnumerable<SqlFragment> ToSql(string prefix, string branchName, IEnumerable<ParameterTag> duplicates)
    {
        int index = 0;
        ArgumentNullException.ThrowIfNull(branchName, nameof(branchName));
        ArgumentNullException.ThrowIfNull(duplicates, nameof(duplicates));


        if (ChildNodes.Count() == 1)
        {
            foreach (SqlFragment fragment in ChildNodes.Single().ToSql(prefix, branchName, duplicates))
                yield return fragment;

            yield break;
        }

        yield return new SqlFragmentText("(");
            foreach (Predicates predicate in ChildNodes)
        {
            if (index > 0)
                yield return new SqlFragmentText($" {_operator} ");
                foreach (SqlFragment fragment in predicate.ToSql($"{prefix}_{index}", branchName, duplicates))
                yield return fragment;
                index++;
        }
        yield return new SqlFragmentText(")");
    }
}
