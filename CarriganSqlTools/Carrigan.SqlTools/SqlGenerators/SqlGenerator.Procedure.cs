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
    /// a table-based model is that you call <c>Procedure</c> rather than an
    /// insert, update, or delete method on the <see cref="SqlGenerator{T}"/>.
    /// <br/><br/>
    /// Only properties that can be publicly read from accessible types are considered;
    /// members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="NullReferenceException">
    /// Thrown if <paramref name="entity"/> is <c>null</c> (or otherwise invalid for reflection-based
    /// property reads) when extracting parameter values.
    /// </exception>
    /// <example>
    /// <para>
    /// 
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// ProcedureExec procedureExec = new()
    /// {
    ///     ValueColumn = "DangIt"
    /// };
    /// SqlQuery query = procedureExecGenerator.Procedure(procedureExec);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// [schema].[UpdateThing]
    /// ]]></code>
    /// </example>
    public SqlQuery Procedure(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        IEnumerable<KeyValuePair<ParameterTag, object>> parameters = ColumnInfo.Select(columns => GetSqlParameterKeyValue(columns, entity));

        return new SqlQuery()
        {
            Parameters = new Dictionary<ParameterTag, object>([.. parameters]),
            QueryText = ProcedureTag,
            CommandType = CommandType.StoredProcedure
        };
    }
}
