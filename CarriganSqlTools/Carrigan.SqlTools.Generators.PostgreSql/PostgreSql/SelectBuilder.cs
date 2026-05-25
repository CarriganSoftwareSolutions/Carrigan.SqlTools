namespace Carrigan.SqlTools.PostgreSql;

public sealed record SelectBuilder<T> : QueryBuilders.SelectBuilderBase<T> where T : class
{
}
