using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using System;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base class for SQL logical operators (e.g., <c>AND</c>, <c>OR</c>) used to combine
/// one or more predicate expressions in <c>WHERE</c> and <c>JOIN</c> clauses.
/// </summary>
public abstract class LogicalOperator : Predicates
{
    private readonly string _operator;
    private readonly IEnumerable<Predicates> _predicates;

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
    /// Thrown when <paramref name="predicates"/> is <c>null</c> or contains no elements.
    /// </exception>
    public LogicalOperator(string op, params IEnumerable<Predicates> predicates) : base(predicates)
    {
        if (predicates.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(predicates), $"{nameof(predicates)} must contain at least one value.");
        _operator = op;
        _predicates = predicates;
    }

    /// <summary>
    /// Generates the SQL fragment represented by this logical operator.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated while traversing the predicate tree. This is appended to parameter
    /// names to keep them unique when duplicates occur.
    /// </param>
    /// <param name="branchName">
    /// the branch prefix that is prepended to the beginning of all of the parameter names in this predicate tree.
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
    internal override IEnumerable<SqlFragment> ToSql(string prefix, string branchName, IEnumerable<ParameterTag> duplicates)
    {
        int index = 0;
        if (_predicates.Count() ==  1)
            foreach(SqlFragment fragment in _predicates.Single().ToSql(prefix, branchName, duplicates))
                yield return fragment;
        else
        {
            yield return new SqlFragmentText("(");
            foreach (Predicates predicate in _predicates)
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
}
