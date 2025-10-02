using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    /// <summary>
    /// Creates an <see cref="SqlQuery"/> that represents a stored procedure call
    /// for the specified data model instance.
    /// </summary>
    /// <param name="entity">
    /// A data model instance that represents the stored procedure to execute,
    /// with its public properties mapped to procedure parameters.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> configured to call the stored procedure,
    /// including parameter values and <see cref="CommandType.StoredProcedure"/>.
    /// </returns>
    /// <remarks>
    /// When creating a data model for a stored procedure, the key distinction from
    /// a table-based model is that you call <c>Procedure</c> instead of an
    /// insert, update, or delete method on the <see cref="SqlGenerator{T}"/>.  
    /// The data model type must be <c>public</c>, and any properties intended
    /// as parameters must be public instance properties with a public getter.
    /// </remarks>
    public SqlQuery Procedure(T entity)
    {
        IEnumerable<KeyValuePair<ParameterTag, object>> parameters = ColumnInfo.Select(columns => GetSqlParameterKeyValue(columns, entity));

        return new SqlQuery()
        {
            Parameters = new Dictionary<ParameterTag, object>([.. parameters]),
            QueryText = ProcedureTag,
            CommandType = System.Data.CommandType.StoredProcedure
        };
    }
}
