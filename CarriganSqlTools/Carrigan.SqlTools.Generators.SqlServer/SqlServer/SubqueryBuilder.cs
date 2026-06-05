namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Represents the <see cref="SubqueryBuilder{T}"/> component.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public sealed record SubqueryBuilder<T> : QueryBuilders.SubqueryBuilderBase<T> where T : class
{
}
