using Carrigan.SqlTools.Fragments;
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
    /// <exception cref="ArgumentNullException">
    /// <paramref name="entity"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown if a mapped column lacks a <see cref="ParameterTag"/> during parameter generation.
    /// This can surface indirectly from
    /// <see cref="GetSqlParameter(ReflectorCache.ColumnInfo, T)"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when one or more procedure parameters resolve to the same <see cref="ParameterTag"/>,
    /// causing a duplicate key during dictionary construction.
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
        IEnumerable<SqlFragmentParameter> parameters = ColumnInfo.Select(column => new SqlFragmentParameter(GetSqlParameter(column, entity)));

        return new SqlQuery()
        {
            Parameters = parameters.GetParameters(Dialect),
            QueryText = ProcedureTag,
            CommandType = CommandType.StoredProcedure
        };
    }
}
