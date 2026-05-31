using Carrigan.SqlTools.Sets;

namespace Carrigan.SqlTools.QueryBuilders;

/// <summary>
/// Represents the options used to build an INSERT query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being inserted.</typeparam>
public abstract record InsertBuilderBase<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InsertBuilderBase{T}"/> record.
    /// </summary>
    /// <param name="records">The records to insert.</param>
    public InsertBuilderBase(params IEnumerable<T> records) =>
        Records = records;

    /// <summary>
    /// Gets or sets the columns to include in the INSERT statement.
    /// </summary>
    public ColumnCollectionBase<T>? InsertColumns { get; set; }

    /// <summary>
    /// Gets or sets the columns to return from the INSERT statement.
    /// </summary>
    public ColumnCollectionBase<T>? ReturnColumns { get; set; }

    /// <summary>
    /// Gets or sets the records to insert.
    /// </summary>
    public required IEnumerable<T> Records { get; set; }

    /// <summary>
    /// Returns a copy of the current query with the specified insert columns.
    /// </summary>
    /// <param name="insertColumns">The columns to include in the INSERT statement.</param>
    /// <returns>A new query instance with the specified insert columns.</returns>
    public InsertBuilderBase<T> WithInsertColumns(ColumnCollectionBase<T>? insertColumns) =>
        this with { InsertColumns = insertColumns };

    /// <summary>
    /// Returns a copy of the current query with the specified return columns.
    /// </summary>
    /// <param name="returnColumns">The columns to return from the INSERT statement.</param>
    /// <returns>A new query instance with the specified return columns.</returns>
    public InsertBuilderBase<T> WithReturnColumns(ColumnCollectionBase<T>? returnColumns) =>
        this with { ReturnColumns = returnColumns };

    /// <summary>
    /// Returns a copy of the current query with the specified records.
    /// </summary>
    /// <param name="records">The records to insert.</param>
    /// <returns>A new query instance with the specified records.</returns>
    public InsertBuilderBase<T> WithRecords(IEnumerable<T> records) =>
        this with { Records = records };
}