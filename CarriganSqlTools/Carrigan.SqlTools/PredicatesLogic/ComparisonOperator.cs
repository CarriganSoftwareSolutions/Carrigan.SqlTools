using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using System;
using System.Reflection;

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
    /// <param name="op">The SQL representation of the comparison o
    public ComparisonOperator(Predicates left, Predicates right, string op) : base([left, right])
    {
        _operator = op;
        _left = left;
        _right = right;
    }

    /// <summary>
    /// Produces the SQL fragment represented by this comparison operator and its operands.
    /// </summary>
    /// <param name="prefix">
    /// A disambiguation prefix accumulated during predicate-tree traversal.
    /// Used to ensure parameter names are unique when duplicates exist.
    /// </param>
    /// <param name="branchName">
    /// the branch prefix that is prepended to the beginning of all of the parameter names in this predicate tree.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied <see cref="ParameterTag"/> values detected as duplicates.
    /// Leaf nodes use this to decide if the <paramref name="prefix"/> should be applied.
    /// </param>
    /// <returns>
    /// A SQL fragment in the form <c>(&lt;left-sql&gt; OP &lt;right-sql&gt;)</c>, e.g.,
    /// <c>([T].[Col] = @Parameter_Col)</c>.
    /// </returns>
    internal override IEnumerable<SqlFragment> ToSql(string prefix, string branchName, IEnumerable<ParameterTag> duplicates)
    {
        yield return new SqlFragmentText("(");

        foreach (SqlFragment fragment in _left.ToSql($"{prefix}_L", branchName, duplicates))
                yield return fragment;

        yield return new SqlFragmentText($" {_operator} ");

        foreach (SqlFragment fragment in _right.ToSql($"{prefix}_R", branchName, duplicates))
            yield return fragment;

        yield return new SqlFragmentText(")");
    }
}