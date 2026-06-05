
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Builds DELETE query options for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being deleted.</typeparam>
public sealed record DeleteBuilder<T> : QueryBuilders.DeleteBuilderBase<T, T>, IQueryBuilder where T : class
{
    /// <summary>
    /// Generates SQL for the builder state.
    /// </summary>
    private readonly SqlGenerator<T> SqlGenerator = new();
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteBuilder{T}"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public DeleteBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);

    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>A <see cref="SqlQuery"/> generated from the current builder state.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Delete(this);
}
