using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PostgreSql;

public interface IQueryBuilder
{
    SqlQuery AsSqlQuery();
}
