using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PostgreSql;

/// <summary>
/// Represents the options used to build a DELETE query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being deleted.</typeparam>
public sealed record DeleteBuilder<T> : QueryBuilders.DeleteBuilderBase<T>  where T : class
{
    /// <summary>
    /// Gets or sets the tables to include in the USING clause.
    /// </summary>
    public IEnumerable<TableTag>? Usings { get; set; }

    /// <summary>
    /// Returns a copy of the current query with the specified USING tables.
    /// </summary>
    /// <param name="usings">The tables to include in the USING clause.</param>
    /// <returns>A new query instance with the specified USING tables.</returns>
    public DeleteBuilder<T> WithUsings(IEnumerable<TableTag>? usings) =>
        this with { Usings = usings };
}
