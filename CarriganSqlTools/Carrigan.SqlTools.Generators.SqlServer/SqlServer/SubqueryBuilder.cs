namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Builds subquery options for the specified model type.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public sealed record SubqueryBuilder<T> : QueryBuilders.SubqueryBuilderBase<T> where T : class
{
}
