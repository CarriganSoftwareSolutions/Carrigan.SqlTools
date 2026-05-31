using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;

namespace Carrigan.SqlTools.QueryBuilders;


/// <summary>
/// Represents the options used to build an UPDATE query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being updated.</typeparam>
/// <typeparam name="joinsT">The model type used as the starting point for the join collection.</typeparam>
/// <remarks>
/// For SQL Server, <typeparamref name="joinsT" /> is usually the same type as <typeparamref name="T" />.
/// For PostgreSQL, <typeparamref name="joinsT" /> should represent one of the source tables in the UPDATE FROM clause.
/// </remarks>
public abstract record UpdateBuilderBase<T, joinsT> 
    where T : class
    where joinsT : class
{
    /// <summary>
    /// Gets or sets the values to apply to the UPDATE statement.
    /// </summary>
    public required T Values { get; set; }

    /// <summary>
    /// Gets or sets the columns to update.
    /// </summary>
    public ColumnCollectionBase<T>? UpdateColumns { get; set; }

    /// <summary>
    /// Gets or sets the joins to include in the UPDATE statement.
    /// </summary>
    public virtual Joins<joinsT>? Joins { get; set; }

    /// <summary>
    /// Gets or sets the predicates used to filter the rows being updated.
    /// </summary>
    public Predicates? Where { get; set; }

    /// <summary>
    /// Returns a copy of the current query with the specified update values.
    /// </summary>
    /// <param name="values">The values to apply to the UPDATE statement.</param>
    /// <returns>A new query instance with the specified update values.</returns>
    public UpdateBuilderBase<T, joinsT> WithValues(T values) =>
        this with { Values = values };

    /// <summary>
    /// Returns a copy of the current query with the specified update columns.
    /// </summary>
    /// <param name="updateColumns">The columns to update.</param>
    /// <returns>A new query instance with the specified update columns.</returns>
    public UpdateBuilderBase<T, joinsT> WithUpdateColumns(ColumnCollectionBase<T>? updateColumns) =>
        this with { UpdateColumns = updateColumns };

    /// <summary>
    /// Returns a copy of the current query with the specified joins.
    /// </summary>
    /// <param name="joins">The joins to include in the UPDATE statement.</param>
    /// <returns>A new query instance with the specified joins.</returns>
    public virtual UpdateBuilderBase<T, joinsT> WithJoins(Joins<joinsT>? joins) =>
        this with { Joins = joins };

    /// <summary>
    /// Returns a copy of the current query with the specified predicates.
    /// </summary>
    /// <param name="predicates">The predicates used to filter the rows being updated.</param>
    /// <returns>A new query instance with the specified predicates.</returns>
    public UpdateBuilderBase<T, joinsT> WithPredicates(Predicates? predicates) =>
        this with { Where = predicates };
}