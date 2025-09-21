using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    /// <summary>
    /// Creates a Sql Query object that represents a stored procedure call.
    /// 
    /// Note:When creating a data model, the contextual difference between a data model for a table and a stored procedure is which method you call from the SQL Generator.
    /// Note: The data model should be public, and any properties you wish to access as columns should be public instance properties with a public getter.
    /// </summary>
    /// <param name="entity">A class that represents a stored procedure call.</param>
    /// <returns>A SqlQuery representing a stored procedure call.</returns>
    public SqlQuery Procedure(T entity)
    {
        IEnumerable<KeyValuePair<ParameterTag, object>> parameters = Columns.Select(columns => GetSqlParameterKeyValue(columns, entity));

        return new SqlQuery()
        {
            Parameters = new Dictionary<ParameterTag, object>([.. parameters]),
            QueryText = ProcedureTag,
            CommandType = System.Data.CommandType.StoredProcedure
        };
    }
}
