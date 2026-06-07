namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Defines a query builder that can materialize its current state as a SQL query.
/// </summary>
public interface IQueryBuilder
{
    SqlQuery AsSqlQuery();
}
