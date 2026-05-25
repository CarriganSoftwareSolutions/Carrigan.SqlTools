using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;

namespace Carrigan.SqlTools.QueryBuilders;


/// <summary>
/// Represents the options used to build an UPDATE query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being updated.</typeparam>
public abstract record UpdateBuilderBase<T> where T : class
{
    /// <summary>
    /// Gets or sets the values to apply to the UPDATE statement.
    /// </summary>
    public required T Values { get; set; }

    /// <summary>
    /// Gets or sets the columns to update.
    /// </summary>
    public ColumnCollection<T>? UpdateColumns { get; set; }

    /// <summary>
    /// Gets or sets the joins to include in the UPDATE statement.
    /// </summary>
    public Joins<T>? Joins { get; set; }

    /// <summary>
    /// Gets or sets the predicates used to filter the rows being updated.
    /// </summary>
    public Predicates? Predicates { get; set; }

    /// <summary>
    /// Returns a copy of the current query with the specified update values.
    /// </summary>
    /// <param name="values">The values to apply to the UPDATE statement.</param>
    /// <returns>A new query instance with the specified update values.</returns>
    public UpdateBuilderBase<T> WithValues(T values) =>
        this with { Values = values };

    /// <summary>
    /// Returns a copy of the current query with the specified update columns.
    /// </summary>
    /// <param name="updateColumns">The columns to update.</param>
    /// <returns>A new query instance with the specified update columns.</returns>
    public UpdateBuilderBase<T> WithUpdateColumns(ColumnCollection<T>? updateColumns) =>
        this with { UpdateColumns = updateColumns };

    /// <summary>
    /// Returns a copy of the current query with the specified joins.
    /// </summary>
    /// <param name="joins">The joins to include in the UPDATE statement.</param>
    /// <returns>A new query instance with the specified joins.</returns>
    public UpdateBuilderBase<T> WithJoins(Joins<T>? joins) =>
        this with { Joins = joins };

    /// <summary>
    /// Returns a copy of the current query with the specified predicates.
    /// </summary>
    /// <param name="predicates">The predicates used to filter the rows being updated.</param>
    /// <returns>A new query instance with the specified predicates.</returns>
    public UpdateBuilderBase<T> WithPredicates(Predicates? predicates) =>
        this with { Predicates = predicates };
}