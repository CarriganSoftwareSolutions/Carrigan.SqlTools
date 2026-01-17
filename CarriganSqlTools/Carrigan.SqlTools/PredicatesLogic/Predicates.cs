using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base type for all SQL predicate nodes used to compose <c>WHERE</c> and <c>JOIN</c> conditions.
/// Supports recursive SQL generation and parameter collection (with automatic de-duplication/prefixing).
/// </summary>
public abstract class Predicates
{
    /// <summary>
    /// Represents the direct children of the current predicate.
    /// </summary>
    internal IEnumerable<Predicates> ChildNodes { get; }

    /// <summary>
    /// Base constructor for all predicate classes.
    /// </summary>
    /// <param name="childPredicates">Represents all child nodes for a given predicate.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="childPredicates"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="childPredicates"/> contains disallowed <c>null</c> values.
    /// </exception>
    protected Predicates(IEnumerable<Predicates> childPredicates)
    {
        ArgumentNullException.ThrowIfNull(childPredicates, nameof(childPredicates));

        ChildNodes = childPredicates.Materialize(NullOptionsEnum.Exception);
    }

    internal IEnumerable<Parameter> DescendantParameters =>
        DescendantNodes.OfType<Parameter>();

    /// <summary>
    /// Represents all parameters that have the same unmodified parameter name.
    /// </summary>
    protected IEnumerable<ParameterTag> DuplicateParameters =>
        DescendantParameters
            .Select(static parameter => parameter.Name)
            .GroupBy(static parameter => parameter)
            .Where(static nameGroup => nameGroup.Count() > 1)
            .Select(static nameGroup => nameGroup.Key);

    /// <summary>
    /// Retrieves all descendant predicates regardless of type.
    /// </summary>
    internal IEnumerable<Predicates> DescendantNodes =>
        GetAllDescendantPredicates(ChildNodes);

    /// <summary>
    /// Retrieves all descendants of type <see cref="ColumnBase"/>.
    /// </summary>
    internal IEnumerable<ColumnBase> DescendantColumns =>
        DescendantNodes.OfType<ColumnBase>();

    /// <summary>
    /// Retrieves all descendant predicates that are not also <see cref="ColumnBase"/> or <see cref="Parameter"/>.
    /// These are the non-leaf operator nodes (e.g., <see cref="LogicalOperator"/>, <see cref="ComparisonOperator"/>).
    /// </summary>
    internal IEnumerable<Predicates> DescendantPredicates =>
        DescendantNodes.Where(static predicate => predicate is not Parameter && predicate is not ColumnBase);

    /// <summary>
    /// Retrieves all direct children of type <see cref="Parameter"/>.
    /// </summary>
    internal IEnumerable<Parameter> ChildParameters =>
        ChildNodes.OfType<Parameter>();

    /// <summary>
    /// Retrieves all direct children of type <see cref="ColumnBase"/>.
    /// </summary>
    internal IEnumerable<ColumnBase> ChildColumns =>
        ChildNodes.OfType<ColumnBase>();

    /// <summary>
    /// Retrieves all direct children that are not also <see cref="ColumnBase"/> or <see cref="Parameter"/>.
    /// These are the non-leaf operator nodes.
    /// </summary>
    internal IEnumerable<Predicates> ChildPredicates =>
        ChildNodes.Where(static predicate => predicate is not Parameter && predicate is not ColumnBase);

    /// <summary>
    /// Generates the SQL fragments for this predicate tree.
    /// </summary>
    /// <remarks>
    /// Before rendering, this method computes duplicate user-supplied parameter names and
    /// passes that set to the recursive <see cref="ToSql(string, string, IEnumerable{ParameterTag})"/> overload,
    /// which may add disambiguating prefixes to produce unique parameter names.
    /// </remarks>
    /// <param name="branchName">
    /// The branch prefix that is prepended to the beginning of all parameter names in this predicate tree.
    /// The leading <c>@</c> is optional and will be ignored.
    /// </param>
    /// <returns>The SQL fragments represented by this predicate tree.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="branchName"/> is <c>null</c>.
    /// </exception>
    internal IEnumerable<SqlFragment> ToSqlFragments(string branchName)
    {
        ArgumentNullException.ThrowIfNull(branchName, nameof(branchName));

        return ToSql(string.Empty, branchName.TrimStart('@'), DuplicateParameters);
    }

    /// <summary>
    /// Recursively generates the SQL fragments for this predicate tree, applying a prefix to
    /// duplicate parameter names to ensure uniqueness.
    /// </summary>
    /// <param name="prefix">
    /// A recursion-built prefix used to disambiguate duplicate parameter names. This is applied
    /// only when the parameter’s base name appears in <paramref name="duplicates"/>.
    /// </param>
    /// <param name="branchName">
    /// the branch prefix that is prepended to the beginning of all of the parameter names in this predicate tree.
    /// </param>
    /// <param name="duplicates">
    /// The set of base <see cref="ParameterTag"/> names that occur more than once within the predicate tree.
    /// </param>
    /// <returns>The SQL fragment for this node.</returns>
    internal abstract IEnumerable<SqlFragment> ToSql(string prefix, string branchName, IEnumerable<ParameterTag> duplicates);

    private static IEnumerable<Predicates> GetAllDescendantPredicates(IEnumerable<Predicates> predicates)
    {
        foreach (Predicates predicate in predicates)
        {
            yield return predicate;

            foreach (Predicates childPredicate in GetAllDescendantPredicates(predicate.ChildNodes))
                yield return childPredicate;
        }
    }
}
