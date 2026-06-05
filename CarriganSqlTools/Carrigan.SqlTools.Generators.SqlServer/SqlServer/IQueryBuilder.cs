using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Defines a query builder that can materialize its current state as a SQL query.
/// </summary>
public interface IQueryBuilder
{
    SqlQuery AsSqlQuery();
}
