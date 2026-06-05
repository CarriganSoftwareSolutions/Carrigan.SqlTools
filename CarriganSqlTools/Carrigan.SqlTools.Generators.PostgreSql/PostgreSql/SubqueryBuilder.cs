namespace Carrigan.SqlTools.PostgreSql;

/// <summary>
/// Represents the <see cref="SubqueryBuilder"/> component.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public sealed record SubqueryBuilder<T> : QueryBuilders.SubqueryBuilderBase<T> where T : class
{
}
