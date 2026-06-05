
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Builds UPDATE query options for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being updated.</typeparam>
public sealed record UpdateBuilder<T> : QueryBuilders.UpdateBuilderBase<T, T>, IQueryBuilder where T : class
{
    /// <summary>
    /// Generates SQL for the builder state.
    /// </summary>
    private readonly SqlGenerator<T> SqlGenerator = new();
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBuilder{T}"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public UpdateBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);
    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>A <see cref="SqlQuery"/> generated from the current builder state.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Update(this);
}
