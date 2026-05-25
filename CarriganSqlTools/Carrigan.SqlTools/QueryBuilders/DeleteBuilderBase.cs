using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.QueryBuilders;

/// <summary>
/// Represents the options used to build a DELETE query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being deleted.</typeparam>
public abstract record DeleteBuilderBase<T> where T : class
{
    /// <summary>
    /// Gets or sets the joins to include in the DELETE statement.
    /// </summary>
    public Joins<T>? Joins { get; set; }

    /// <summary>
    /// Gets or sets the predicates used to filter the rows being deleted.
    /// </summary>
    public Predicates? Where { get; set; }

    /// <summary>
    /// Returns a copy of the current query with the specified joins.
    /// </summary>
    /// <param name="joins">The joins to include in the DELETE statement.</param>
    /// <returns>A new query instance with the specified joins.</returns>
    public DeleteBuilderBase<T> WithJoins(Joins<T>? joins) =>
        this with { Joins = joins };

    /// <summary>
    /// Returns a copy of the current query with the specified predicates.
    /// </summary>
    /// <param name="predicates">The predicates used to filter the rows being deleted.</param>
    /// <returns>A new query instance with the specified predicates.</returns>
    public DeleteBuilderBase<T> WithPredicates(Predicates? predicates) =>
        this with { Where = predicates };
}