namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Base type for SELECT projection containers.
/// Provides an abstraction over a single <see cref="SelectTag"/> or a collection of them.
/// </summary>
/// <remarks>
/// Derived types must expose the contained projections, render them to SQL, surface the
/// associated <see cref="TableTag"/> values, and report basic presence/emptiness semantics.
/// </remarks>
public abstract class SelectTagsBase
{
    /// <summary>
    /// Returns all <see cref="SelectTag"/> items represented by this instance.
    /// </summary>
    /// <returns>An enumeration of <see cref="SelectTag"/> values.</returns>
    public abstract IEnumerable<SelectTag> All();

    /// <summary>
    /// Returns the SQL text for all select tags represented by this instance.
    /// </summary>
    /// <remarks>
    /// For a single <see cref="SelectTag"/>, this is just that tag’s SQL.
    /// For a collection, tags are typically joined by <c>", "</c>.
    /// </remarks>
    /// <returns>The SQL text represented by this instance.</returns>
    public abstract string ToSql();

    /// <summary>
    /// Gets all distinct <see cref="TableTag"/> values referenced by the select tags in this instance.
    /// </summary>
    /// <returns>An enumeration of unique <see cref="TableTag"/> values.</returns>
    internal abstract IEnumerable<TableTag> GetTableTags();

    /// <summary>
    /// Determines if this instance contains any actual SelectTags
    /// For SelectTag, this should always be true.
    /// For SelectTags, this will be true if the underlying Enumeration is not empty.
    /// </summary>
    /// <returns>
    /// True if this instance contains any actual SelectTags
    /// For SelectTag, this should always be true.
    /// For SelectTags, this will be true if the underlying Enumeration is not empty.
    /// </returns>
    public abstract bool Any();

    /// <summary>
    /// Indicates whether this instance represents no select tags.
    /// </summary>
    /// <remarks>
    /// A single <see cref="SelectTag"/> should return <c>false</c>.
    /// A collection returns <c>true</c> only if it is empty.
    /// </remarks>
    /// <returns><c>true</c> if no items exist; otherwise, <c>false</c>.</returns>
    public abstract bool Empty();
}
