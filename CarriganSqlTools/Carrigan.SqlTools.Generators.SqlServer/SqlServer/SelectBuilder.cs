
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Represents the <see cref="SelectBuilder"/> component.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public sealed record SelectBuilder<T> : QueryBuilders.SelectBuilderBase<T>, IQueryBuilder where T : class
{
    private readonly SqlGenerator<T> SqlGenerator = new();
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectBuilder"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public SelectBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);

    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>The result of the AsSqlQuery operation.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Select(this);
}
