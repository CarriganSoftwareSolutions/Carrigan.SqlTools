
namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Represents the options used to build a DELETE query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being deleted.</typeparam>
public sealed record DeleteBuilder<T> : QueryBuilders.DeleteBuilderBase<T>  where T : class 
{
}
