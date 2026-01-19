using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer;

internal static class SqlQueryExtensions
{
    /// <summary>
    /// Converts the parameters in an <see cref="SqlQuery"/> into an
    /// <see cref="IEnumerable{SqlParameter}"/> for consumption by
    /// <see cref="Commands"/> or <see cref="CommandsAsync"/>.
    /// </summary>
    /// <param name="query">The <see cref="SqlQuery"/> containing parameter metadata and values.</param>
    /// <returns>
    /// An <see cref="IEnumerable{SqlParameter}"/> mapping each <see cref="ParameterTag"/>
    /// in the query to a fully populated <see cref="SqlParameter"/> instance.
    /// </returns>
    /// <remarks>
    /// <para>
    /// For each parameter, this method:
    /// </para>
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///         Creates a <see cref="SqlParameter"/> using the parameter name and value.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///         Determines the correct SqlDbType using the associated
    ///         <see cref="SqlTypeDefinition"/> if present, or derives one automatically
    ///         via <see cref="SqlTypeDefinition"/> if no explicit SQL type was provided.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///         Applies size, precision, scale, or <c>MAX</c> semantics based on the
    ///         <see cref="SqlTypeDefinition"/> assigned to the corresponding
    ///         <see cref="ParameterTag"/>.
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>

    internal static IEnumerable<SqlParameter> GetParameterCollection(this SqlQuery query)
    {
        static SqlParameter GetSqlParameter(ParameterTag parameter, object value)
        {
            object valueToUse = value is null ? DBNull.Value : value;

            SqlParameter sqlParameter = new(parameter, valueToUse)
            {
                SqlDbType = parameter.SqlType?.Type ?? (new SqlTypeDefinition(valueToUse)).Type
            };

            if (parameter.SqlType is not null)
            {
                if (parameter.SqlType.UseMax)
                    sqlParameter.Size = -1;
                else
                {
                    if (parameter.SqlType.Size is not null)
                        sqlParameter.Size = parameter.SqlType.Size.Value;
                    if (parameter.SqlType.Precision is not null)
                        sqlParameter.Precision = parameter.SqlType.Precision.Value;
                    if (parameter.SqlType.Scale is not null)
                        sqlParameter.Scale = parameter.SqlType.Scale.Value;
                }
            }

            return sqlParameter;
        }

        return query
                    .Parameters
                    .AsEnumerable()
                    .Select(parameter => GetSqlParameter(parameter.Key, parameter.Value));
    }
}
