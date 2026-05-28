
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Represents the options used to build an UPDATE query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being updated.</typeparam>
public sealed record UpdateBuilder<T> : QueryBuilders.UpdateBuilderBase<T, T>, IQueryBuilder where T : class
{
    private readonly SqlGenerator<T> SqlGenerator = new();
    public UpdateBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Update(this);
}
