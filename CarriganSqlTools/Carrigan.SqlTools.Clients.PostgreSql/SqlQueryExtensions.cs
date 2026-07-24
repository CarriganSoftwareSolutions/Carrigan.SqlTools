using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Carrigan.SqlTools.Clients.PostgreSql;

/// <summary>
/// Provides internal helpers for binding rendered SQL query parameters to PostgreSQL commands.
/// </summary>
internal static class SqlQueryExtensions
{
    /// <summary>
    /// Converts the parameters in an <see cref="SqlQuery"/> into an
    /// <see cref="IEnumerable{NpgsqlParameter}"/> for consumption by
    /// <see cref="Commands"/> or <see cref="CommandsAsync"/>.
    /// </summary>
    /// <param name="query">The <see cref="SqlQuery"/> containing parameter metadata and values.</param>
    /// <returns>
    /// An <see cref="IEnumerable{NpgsqlParameter}"/> mapping each <see cref="ParameterTag"/>
    /// in the query to a fully populated <see cref="NpgsqlParameter"/> instance.
    /// </returns>
    /// <remarks>
    /// <para>
    /// For each parameter, this method:
    /// </para>
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///         Creates a <see cref="NpgsqlParameter"/> using the parameter name and value.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///         Determines the correct <see cref="NpgsqlDbType"/> using the associated
    ///         <see cref="FieldProperties"/> if present, or derives one automatically
    ///         via <see cref="ISqlDialects.FromClrValue"/> if no explicit SQL type was provided.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///         Applies size, precision, scale, or <c>MAX</c> semantics based on the
    ///         <see cref="FieldProperties"/> assigned to the corresponding
    ///         <see cref="SqlFragmentParameter"/>.
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    internal static IEnumerable<NpgsqlParameter> GetParameterCollection(this SqlQuery query)
    {
        NpgsqlParameter GetSqlParameter(SqlFragmentParameter parameter)
        {
            object valueToUse = query.Dialect.ValueConversion(parameter.Value);
            FieldProperties fieldProperties = parameter.FieldProperties is null
                ? query.Dialect.FromClrValue(valueToUse)
                : parameter.FieldProperties;

            NpgsqlParameter sqlParameter = new()
            {
                NpgsqlValue = valueToUse,
                NpgsqlDbType = FieldPropertiesToNpgsqlDbType(fieldProperties),
                IsNullable = fieldProperties.IsNullable,

            };
            if (fieldProperties.IsMax == true)
                sqlParameter.Size = -1;
            else if (fieldProperties.Length is not null)
                sqlParameter.Size = fieldProperties.Length.Value;

            if (fieldProperties.Precision is not null)
                sqlParameter.Precision = fieldProperties.Precision.Value;

            if (fieldProperties.Scale is not null)
                sqlParameter.Scale = fieldProperties.Scale.Value;

            if (fieldProperties.FractionalSecondsPrecision is not null)
                sqlParameter.Scale = fieldProperties.FractionalSecondsPrecision.Value;

            return sqlParameter;
        }

        return query
                    .Parameters
                    .AsEnumerable()
                    .Select(parameter => GetSqlParameter(parameter));
    }

    /// <summary>
    /// Maps a <see cref="FieldProperties"/> instance to the appropriate <see cref="NpgsqlDbType"/>
    /// </summary>
    /// <param name="fieldProperties">The field properties to map.</param>
    /// <returns>The corresponding <see cref="NpgsqlDbType"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the provider type name is not supported.</exception>
    private static NpgsqlDbType FieldPropertiesToNpgsqlDbType(FieldProperties fieldProperties)
    {
        ArgumentNullException.ThrowIfNull(fieldProperties);

        string providerTypeName = fieldProperties.ProviderTypeName?.Trim().ToUpperInvariant()
            ?? throw new ArgumentOutOfRangeException(nameof(fieldProperties), fieldProperties.ProviderTypeName, "PostgreSQL provider type name cannot be null.");

        NpgsqlDbType npgsqlDbType = providerTypeName switch
        {
            "UUID" => NpgsqlDbType.Uuid,

            "CHAR" => NpgsqlDbType.Char,
            "CHARACTER" => NpgsqlDbType.Char,
            "VARCHAR" => NpgsqlDbType.Varchar,
            "CHARACTER VARYING" => NpgsqlDbType.Varchar,
            "TEXT" => NpgsqlDbType.Text,

            "BYTEA" => NpgsqlDbType.Bytea,

            "BOOL" => NpgsqlDbType.Boolean,
            "BOOLEAN" => NpgsqlDbType.Boolean,

            "SMALLINT" => NpgsqlDbType.Smallint,
            "INT2" => NpgsqlDbType.Smallint,

            "INTEGER" => NpgsqlDbType.Integer,
            "INT" => NpgsqlDbType.Integer,
            "INT4" => NpgsqlDbType.Integer,

            "BIGINT" => NpgsqlDbType.Bigint,
            "INT8" => NpgsqlDbType.Bigint,

            "REAL" => NpgsqlDbType.Real,
            "FLOAT4" => NpgsqlDbType.Real,

            "DOUBLE PRECISION" => NpgsqlDbType.Double,
            "FLOAT8" => NpgsqlDbType.Double,

            "FLOAT" => fieldProperties.Precision is <= 24
                ? NpgsqlDbType.Real
                : NpgsqlDbType.Double,

            "NUMERIC" => NpgsqlDbType.Numeric,
            "DECIMAL" => NpgsqlDbType.Numeric,
            "MONEY" => NpgsqlDbType.Money,

            "DATE" => NpgsqlDbType.Date,

            "TIME" => NpgsqlDbType.Time,
            "TIME WITHOUT TIME ZONE" => NpgsqlDbType.Time,
            "TIME WITH TIME ZONE" => NpgsqlDbType.TimeTz,

            "INTERVAL" => NpgsqlDbType.Interval,

            "TIMESTAMP" => NpgsqlDbType.Timestamp,
            "TIMESTAMP WITHOUT TIME ZONE" => NpgsqlDbType.Timestamp,
            "TIMESTAMP WITH TIME ZONE" => NpgsqlDbType.TimestampTz,

            "XML" => NpgsqlDbType.Xml,
            "JSON" => NpgsqlDbType.Json,
            "JSONB" => NpgsqlDbType.Jsonb,

            "BIT" => NpgsqlDbType.Bit,
            "BIT VARYING" => NpgsqlDbType.Varbit,
            "VARBIT" => NpgsqlDbType.Varbit,

            "UNKNOWN" => NpgsqlDbType.Unknown,

            _ => throw new ArgumentOutOfRangeException(nameof(fieldProperties), fieldProperties.ProviderTypeName, "Unsupported PostgreSQL provider type name.")
        };

        return fieldProperties.IsArray is true
            ? NpgsqlDbType.Array | npgsqlDbType
            : npgsqlDbType;
    }
}
