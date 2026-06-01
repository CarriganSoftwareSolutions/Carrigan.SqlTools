using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer;

public interface IQueryBuilder
{
    SqlQuery AsSqlQuery();
}
