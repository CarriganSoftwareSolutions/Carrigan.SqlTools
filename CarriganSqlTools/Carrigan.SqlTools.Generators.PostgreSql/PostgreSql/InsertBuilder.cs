namespace Carrigan.SqlTools.PostgreSql;

/// <summary>
/// Represents an SQL <c>INSERT</c> query for a specific entity type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The entity/model type that defines the table into which data will be inserted. 
/// </typeparam>
public sealed record InsertBuilder<T> : QueryBuilders.InsertBuilderBase<T> where T : class
{
}
