
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Builds INSERT query options for a specific entity type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The entity/model type that defines the table into which data will be inserted.
/// </typeparam>
public sealed record InsertBuilder<T> : QueryBuilders.InsertBuilderBase<T>, IQueryBuilder where T : class
{
    /// <summary>
    /// Generates SQL for the builder state.
    /// </summary>
    private readonly SqlGenerator<T> SqlGenerator = new();
    /// <summary>
    /// Initializes a new instance of the <see cref="InsertBuilder{T}"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public InsertBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);
    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>A <see cref="SqlQuery"/> generated from the current builder state.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Insert(this);
}
