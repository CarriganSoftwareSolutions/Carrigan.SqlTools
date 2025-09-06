namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    public SqlQuery Procedure(T entity)
    {
        IEnumerable<KeyValuePair<string, object>> parameters = _Properties.Select(property => GetSqlParameterKeyValue(property, true, entity));

        return new SqlQuery()
        {
            Parameters = new Dictionary<string, object>([.. parameters]),
            QueryText = $"{_ProcedureName}",
            CommandType = System.Data.CommandType.StoredProcedure
        };
    }
}
