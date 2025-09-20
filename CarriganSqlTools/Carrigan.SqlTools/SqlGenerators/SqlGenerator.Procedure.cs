using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    //TODO: Needs additional documentation.
    /// <summary>
    /// 
    /// Note: The data model should be public, and any properties you wish to access as columns should be public instance properties with a public getter.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
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
