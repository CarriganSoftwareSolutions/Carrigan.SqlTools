
namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Represents the options used to build an UPDATE query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being updated.</typeparam>
public sealed record UpdateBuilder<T> : QueryBuilders.UpdateBuilderBase<T> where T : class
{
}
