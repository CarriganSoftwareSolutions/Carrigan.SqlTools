using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using Microsoft.Data.SqlClient;
using System.Data;

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
    ///         <see cref="FieldProperties"/> if present, or derives one automatically
    ///         via <see cref="SqlServerTypesProvider.FromClrType"/> if no explicit SQL type was provided.
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

    internal static IEnumerable<SqlParameter> GetParameterCollection(this SqlQuery query)
    {
        static SqlParameter GetSqlParameter(SqlFragmentParameter parameter)
        {
            SqlServerDialect dialect = new();
            object valueToUse = dialect.ValueConversion(parameter.Value);
            FieldProperties fieldProperties = parameter.FieldProperties is null
                ? SqlServerTypesProvider.FromClrType(valueToUse.GetType())
                : parameter.FieldProperties;


            SqlParameter sqlParameter = new()
            {
                Value = valueToUse,
                ParameterName = parameter.ParameterTag,
                SqlDbType = FieldPropertiesToSqlDbType(fieldProperties),
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
                    .Select(static parameter => GetSqlParameter(parameter));
    }


    private static SqlDbType FieldPropertiesToSqlDbType(FieldProperties fieldProperties)
    {
        ArgumentNullException.ThrowIfNull(fieldProperties);

        return fieldProperties.ProviderTypeName?.Trim().ToUpperInvariant() switch
        {
            "BIGINT" => SqlDbType.BigInt,
            "BINARY" => SqlDbType.Binary,
            "BIT" => SqlDbType.Bit,
            "CHAR" => SqlDbType.Char,
            "CURSOR" => SqlDbType.Variant,
            "DATE" => SqlDbType.Date,
            "DATETIME" => SqlDbType.DateTime,
            "DATETIME2" => SqlDbType.DateTime2,
            "DATETIMEOFFSET" => SqlDbType.DateTimeOffset,
            "DECIMAL" => SqlDbType.Decimal,
            "FLOAT" => SqlDbType.Float,
            "GEOGRAPHY" => SqlDbType.Udt,
            "GEOMETRY" => SqlDbType.Udt,
            "HIERARCHYID" => SqlDbType.Udt,
            "IMAGE" => SqlDbType.Image,
            "INT" => SqlDbType.Int,
            "JSON" => SqlDbType.NVarChar,
            "MONEY" => SqlDbType.Money,
            "NCHAR" => SqlDbType.NChar,
            "NTEXT" => SqlDbType.NText,
            "NUMERIC" => SqlDbType.Decimal,
            "NVARCHAR" => SqlDbType.NVarChar,
            "REAL" => SqlDbType.Real,
            "ROWVERSION" => SqlDbType.Timestamp,
            "SMALLDATETIME" => SqlDbType.SmallDateTime,
            "SMALLINT" => SqlDbType.SmallInt,
            "SMALLMONEY" => SqlDbType.SmallMoney,
            "SQL_VARIANT" => SqlDbType.Variant,
            "TABLE" => SqlDbType.Structured,
            "TEXT" => SqlDbType.Text,
            "TIME" => SqlDbType.Time,
            "TIMESTAMP" => SqlDbType.Timestamp,
            "TINYINT" => SqlDbType.TinyInt,
            "UNIQUEIDENTIFIER" => SqlDbType.UniqueIdentifier,
            "VARBINARY" => SqlDbType.VarBinary,
            "VARCHAR" => SqlDbType.VarChar,
            "VECTOR" => SqlDbType.VarBinary,
            "XML" => SqlDbType.Xml,
            _ => throw new ArgumentOutOfRangeException(nameof(fieldProperties), fieldProperties.ProviderTypeName, "Unsupported SQL Server provider type name.")
        };
    }
}
