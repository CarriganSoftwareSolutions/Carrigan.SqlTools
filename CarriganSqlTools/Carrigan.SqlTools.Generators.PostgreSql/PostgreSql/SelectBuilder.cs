using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PostgreSql;

public sealed record SelectBuilder<T> : QueryBuilders.SelectBuilderBase<T>, IQueryBuilder where T : class
{
    private readonly SqlGenerator<T> SqlGenerator = new();

    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Select(this);
}
