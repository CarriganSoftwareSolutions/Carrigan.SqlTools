using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using System;

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
    /// Base constructor for all predicate classes
    /// </summary>
    /// <param name="childPredicates">represents all child nodes for a given predicate</param>
    protected Predicates(IEnumerable<Predicates> childPredicates) =>
        ChildNodes = childPredicates;

    /// <summary>
    /// Retrieves all descendants of type <see cref="Parameter"/>.
    /// </summary>
    internal IEnumerable<Parameter> DescendantParameters =>
        DescendantNodes.OfType<Parameter>();

    /// <summary>
    /// Represents all parameters that have the same unmodified parameter name.
    /// </summary>
    protected IEnumerable<ParameterTag> DuplicateParameters => DescendantParameters
        .Select(parameter => parameter.Name)
        .GroupBy(parameter => parameter)
        .Where(nameGroup => nameGroup.Count() > 1)
        .Select(nameGroup => nameGroup.Key);


    /// <summary>
    /// Retrieves all descendants predicates regardless of type.
    /// </summary>
    internal IEnumerable<Predicates> DescendantNodes =>
        GetAllDescendentPredicates(ChildNodes);

    /// <summary>
    /// Retrieves all descendants of type <see cref="ColumnBase"/>.
    /// </summary>
    internal IEnumerable<ColumnBase> DescendantColumns =>
        DescendantNodes.OfType<ColumnBase>();

    /// <summary>
    /// Retrieves all descendants predicates that are not also <see cref="ColumnBase"/> or  <see cref="Parameter"/> (the leaf nodes).
    /// </summary>
    internal IEnumerable<Predicates> DescendantPredicates =>
        DescendantNodes.Where(predicate => predicate is not Parameter && predicate is not ColumnBase);

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
    /// Retrieves all direct children predicates that are not also columns or parameters (the leaf nodes).
    /// </summary>
    internal IEnumerable<Predicates> ChildPredicates =>
        ChildNodes.Where(predicate => predicate is not Parameter && predicate is not ColumnBase);

    /// <summary>
    /// Generates the SQL fragment for this predicate tree.
    /// </summary>
    /// <remarks>
    /// Before rendering, this method computes duplicate user-supplied parameter names and
    /// passes that set to the recursive <see cref="ToSql(string, IEnumerable{ParameterTag})"/> overload,
    /// which may add disambiguating prefixes to produce unique parameter names.
    /// </remarks>
    /// <returns>The SQL fragment represented by this predicate tree.</returns>
    internal IEnumerable<SqlFragment> ToSqlFragments(string branchName) =>
        ToSql(string.Empty, branchName.TrimStart('@'), DuplicateParameters);

    /// <summary>
    /// Recursively generates the SQL fragment for this predicate tree, applying a prefix to
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
    //internal abstract string ToSql(string prefix, IEnumerable<ParameterTag> duplicates);
    internal abstract IEnumerable<SqlFragment> ToSql(string prefix, string branchName, IEnumerable<ParameterTag> duplicates);


    private static IEnumerable<Predicates> GetAllDescendentPredicates(IEnumerable<Predicates> predicates)
    {
        foreach (Predicates predicate in predicates)
        {
            yield return predicate;

            foreach (Predicates childPredicate in GetAllDescendentPredicates(predicate.ChildPredicates))
                yield return childPredicate;
        }
    }


}
