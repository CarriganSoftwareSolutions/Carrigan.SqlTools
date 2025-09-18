namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    //TODO: Needs aditional documentation.
    /// <summary>
    /// 
    /// Note: The data model should be public, and any properties you wish to access as columns should be public instance properties with a public getter.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public SqlQuery Procedure(T entity)
    {
        IEnumerable<KeyValuePair<string, object>> parameters = Properties.Select(property => GetSqlParameterKeyValue(property, true, entity));

        return new SqlQuery()
        {
            Parameters = new Dictionary<string, object>([.. parameters]),
            QueryText = ProcedureTag,
            CommandType = System.Data.CommandType.StoredProcedure
        };
    }
}
