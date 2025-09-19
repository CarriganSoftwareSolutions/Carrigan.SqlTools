using Carrigan.SqlTools.Query;
using Carrigan.SqlTools.Tags;
using System.Data;

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
        IEnumerable<KeyValuePair<ParameterTag, object>> parameters = Columns.Select(columns => GetSqlParameterKeyValue(columns, true, entity));

        return new SqlQuery(ProcedureTag, new Dictionary<ParameterTag, object>([.. parameters]), CommandType.StoredProcedure);
    }
}
