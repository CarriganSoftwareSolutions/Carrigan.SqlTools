
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Represents the options used to build a DELETE query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being deleted.</typeparam>
public sealed record DeleteBuilder<T> : QueryBuilders.DeleteBuilderBase<T>, IQueryBuilder where T : class
{
    private readonly SqlGenerator<T> SqlGenerator = new();
    public DeleteBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Delete(this);
}
