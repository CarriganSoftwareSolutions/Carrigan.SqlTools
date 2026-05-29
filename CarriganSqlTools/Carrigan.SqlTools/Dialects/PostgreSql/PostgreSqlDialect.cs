using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.Xml;
using System.Xml.Linq;

//IGNORE SPELLING: Npgsql

namespace Carrigan.SqlTools.Dialects.PostgreSql;

/// <summary>
/// Provides SQL dialect-specific formatting and rendering logic for PostgreSQL.
/// </summary>
public class PostgreSqlDialect : ISqlDialects
{
    /// <summary>
    /// Encloses the specified identifier in PostgreSQL double quotes.
    /// Embedded double quotes are escaped by doubling them.
    /// </summary>
    public string QuoteIdentifier(string identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier);

        return $"\"{identifier.Replace("\"", "\"\"")}\"";
    }

    /// <summary>
    /// Generates a string representation of the specified database procedure, optionally within a given schema.
    /// </summary>
    /// <param name="procedure">The procedure tag to render. Cannot be null or empty.</param>
    /// <returns>A string containing the rendered representation of the specified procedure.</returns>
    public string RenderProcedureTag(ProcedureTag procedure) =>
        procedure.SchemaName.IsNotNullOrEmpty()
            ? $"{QuoteIdentifier(procedure.SchemaName)}.{QuoteIdentifier(procedure.ProcedureName)}"
            : QuoteIdentifier(procedure.ProcedureName);

    /// <summary>
    /// Generates a string representation of the specified PostgreSQL table,
    /// optionally qualified by schema.
    /// </summary>
    public string RenderTable(SchemaName? schemaName, TableName tableName) =>
        schemaName.IsNotNullOrEmpty()
            ? $"{QuoteIdentifier(schemaName)}.{QuoteIdentifier(tableName)}"
            : QuoteIdentifier(tableName);

    /// <summary>
    /// Renders the fully qualified PostgreSQL column name.
    /// </summary>
    public string RenderColumn(TableTag tableTag, ColumnName columnName, bool includeTable = true) =>
        includeTable && tableTag.ToString().IsNotNullOrEmpty()
            ? $"{tableTag.ToSql(this)}.{QuoteIdentifier(columnName)}"
            : QuoteIdentifier(columnName);

    /// <summary>
    /// Generates SQL fragments for an INSERT statement that returns inserted values
    /// by appending PostgreSQL's RETURNING clause.
    /// </summary>
    public IEnumerable<ISqlFragment> GetInsertReturningFragments<T>(IEnumerable<ISqlFragment> insertIntoFragments, IEnumerable<ISqlFragment> insertValuesFragments, IEnumerable<ColumnInfo> columnInfo)
    {
        ArgumentNullException.ThrowIfNull(insertIntoFragments);
        ArgumentNullException.ThrowIfNull(insertValuesFragments);
        ArgumentNullException.ThrowIfNull(columnInfo);

        ColumnInfo[] columns = [.. columnInfo];

        IEnumerable<ISqlFragment> fragments = insertIntoFragments
            .Append(ISqlFragment.NewLine)
            .Concat(insertValuesFragments);

        if (columns.Length == 0)
        {
            return fragments.Concat([ISqlFragment.Semicolon, ISqlFragment.NewLine]);
        }
        else
        {
            return fragments
                .Append(ISqlFragment.NewLine)
                .Append(new SqlFragmentText(ReturningColumns<T>(columns)))
                .Concat([ISqlFragment.Semicolon, ISqlFragment.NewLine]);
        }
    }

    private string ReturningColumns<T>(IEnumerable<ColumnInfo> columnInfo) =>
        $"RETURNING {string.Join(", ", columnInfo.Select(ReturningColumn<T>))}";

    private string ReturningColumn<T>(ColumnInfo columnInfo)
    {
        string resultColumnName = InvocationReflectorCache<T>.GetResultColumnName(columnInfo.PropertyInfo);
        string columnName = QuoteIdentifier(columnInfo.ColumnName);

        if (resultColumnName != columnInfo.ColumnName)
        {
            return $"{columnName} AS {QuoteIdentifier(resultColumnName)}";
        }

        return columnName;
    }

    /// <summary>
    /// Generates a PostgreSQL LIMIT/OFFSET paging clause.
    /// </summary>
    public ISqlFragment RenderPaging(PagingBase paging) =>
        new SqlFragmentText
        (
            (paging.Offset, paging.Next) switch
            {
                (0u, 0u) => string.Empty,
                (0u, _) => $"LIMIT {paging.Next}",
                (_, 0u) => $"OFFSET {paging.Offset}",
                _ => $"LIMIT {paging.Next} OFFSET {paging.Offset}"
            }
        );

    /// <summary>
    /// Renders PostgreSQL's native positional parameter name.
    /// PostgreSQL/Npgsql positional parameters are 1-origin: $1, $2, $3, etc.
    /// </summary>
    public string RenderFinalParameterName(string baseParameterName, int parameterIndex) =>
        $"${parameterIndex}";

    /// <summary>
    /// Returns the default PostgreSQL field properties for a CLR type.
    /// </summary>
    public FieldProperties GetDefaultFieldPropertiesByClrType(Type type) =>
        PostgreSqlTypesProvider.FromClrType(type);

    /// <summary>
    /// Generates the PostgreSQL declaration for a field based on the provided field properties.
    /// </summary>
    public string RenderFieldProperties(FieldProperties fieldProperties)
    {
        ArgumentNullException.ThrowIfNull(fieldProperties);

        if (fieldProperties.ProviderTypeName.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        string declaration = fieldProperties.ProviderTypeName.ToUpperInvariant();

        if (declaration == "VECTOR" && fieldProperties.Length is not null)
        {
            declaration += $"({fieldProperties.Length})";
        }
        else if (fieldProperties.Precision is not null && fieldProperties.Scale is not null)
        {
            declaration += $"({fieldProperties.Precision}, {fieldProperties.Scale})";
        }
        else if (fieldProperties.Precision is not null)
        {
            declaration += $"({fieldProperties.Precision})";
        }
        else if (fieldProperties.FractionalSecondsPrecision is not null)
        {
            declaration = RenderTemporalPrecision(declaration, fieldProperties.FractionalSecondsPrecision.Value);
        }
        else if (fieldProperties.Length is not null && RequiresLengthDeclaration(declaration))
        {
            declaration += $"({fieldProperties.Length})";
        }

        return $"{declaration} {(fieldProperties.IsNullable ? "NULL" : "NOT NULL")}";
    }

    /// <summary>
    /// Determines whether the specified PostgreSQL provider type name requires a length declaration when rendering field properties.
    /// </summary>
    /// <param name="providerTypeName">
    /// The PostgreSQL provider type name to evaluate, e.g., "VARCHAR", "CHAR", "BIT", etc.
    /// </param>
    /// <returns>
    /// <c>true</c> if the provider type name requires a length declaration; otherwise, <c>false</c>.
    /// </returns>
    private static bool RequiresLengthDeclaration(string providerTypeName) =>
        providerTypeName is
            "CHAR" or
            "CHARACTER" or
            "VARCHAR" or
            "CHARACTER VARYING" or
            "BIT" or
            "BIT VARYING" or
            "VARBIT";

    /// <summary>
    /// Renders the PostgreSQL declaration for a temporal field (TIME or TIMESTAMP) with the specified fractional seconds precision.
    /// </summary>
    /// <param name="declaration">
    /// The base declaration for the temporal type, e.g., "TIME WITH TIME ZONE", "TIMESTAMP WITHOUT TIME ZONE", etc.
    /// </param>
    /// <param name="fractionalSecondsPrecision">
    /// The fractional seconds precision for the temporal type.
    /// </param>
    /// <returns>
    /// The rendered declaration for the temporal type with the specified fractional seconds precision.
    /// </returns>
    private static string RenderTemporalPrecision(string declaration, byte fractionalSecondsPrecision) =>
        declaration switch
        {
            "TIME WITH TIME ZONE" => $"TIME({fractionalSecondsPrecision}) WITH TIME ZONE",
            "TIME WITHOUT TIME ZONE" => $"TIME({fractionalSecondsPrecision}) WITHOUT TIME ZONE",
            "TIMESTAMP WITH TIME ZONE" => $"TIMESTAMP({fractionalSecondsPrecision}) WITH TIME ZONE",
            "TIMESTAMP WITHOUT TIME ZONE" => $"TIMESTAMP({fractionalSecondsPrecision}) WITHOUT TIME ZONE",
            _ => $"{declaration}({fractionalSecondsPrecision})"
        };

    /// <summary>
    /// Performs PostgreSQL parameter value conversions.
    /// </summary>
    public object ValueConversion(object? value)
    {
        if (value == null)
        {
            return DBNull.Value;
        }
        else if (value is XDocument xDocument)
        {
            return xDocument.ToString();
        }
        else if (value is XmlDocument xmlDocument)
        {
            return ((object?)xmlDocument.OuterXml) ?? DBNull.Value;
        }
        else
        {
            return value;
        }
    }

    /// <summary>
    /// Returns the PostgreSQL-specific symbol used to represent the logical XOR operator in SQL expressions.
    /// </summary>
    /// <returns>
    /// A string containing the PostgreSQL symbol for the logical XOR operator, which is "#".
    /// </returns>
    public ISqlFragment GetXOrSymbol() =>
        new SqlFragmentText("#");

    /// <summary>
    /// Returns the appropriate SQL fragment for a LIKE operator based on the specified case sensitivity preference for PostgreSQL.
    /// </summary>
    /// <param name="isCaseSensitive">
    /// A value indicating whether the LIKE operator should be case-sensitive.
    /// </param>
    /// <returns>
    /// An SQL fragment representing the appropriate LIKE operator for PostgreSQL.
    /// </returns>
    public ISqlFragment GetDialectLike(bool? isCaseSensitive = null)
    {
        if (isCaseSensitive is null || isCaseSensitive.Value)
            return new SqlFragmentText("LIKE");
        else
            return new SqlFragmentText("ILIKE");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    /// <c>false</c>, as PostgreSQL does not support fully qualified sets in UPDATE statements.
    /// </returns>
    public bool DoesUpdateSupportsFullyQualifiedSets() =>
        false;

    /// <summary>
    /// Normalizes a <see cref="DateTimeOffset"/> value to UTC by converting it to Universal Time.
    /// </summary>
    /// <param name="dateTimeOffset">
    /// The <see cref="DateTimeOffset"/> value to normalize. If null, the method returns null.
    /// </param>
    /// <returns>
    /// A normalized <see cref="DateTimeOffset"/> value in UTC, or null if the input was null.
    /// </returns>
    public DateTimeOffset? NormalizeTimeZone(DateTimeOffset? dateTimeOffset) =>
        dateTimeOffset?.ToUniversalTime();

    /// <summary>
    /// Normalizes a <see cref="DateTime"/> value by converting it to UTC if its Kind is Local or Utc, and then removing
    /// the Kind information to set it to Unspecified.
    /// Local       -> converts to UTC time, then removes Kind
    /// Utc         -> keeps the same clock value, then removes Kind
    /// Unspecified -> leaves the clock value alone
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to normalize. If null, the method returns null.
    /// </param>
    /// <returns>
    /// A normalized <see cref="DateTime"/> value with Kind set to Unspecified, or null if the input was null.
    /// The clock value is adjusted based on the original Kind as follows
    /// Local       -> converts to UTC time, then removes Kind
    /// Utc         -> keeps the same clock value, then removes Kind
    /// Unspecified -> leaves the clock value alone
    /// </returns>
    public DateTime? NormalizeTimeZone(DateTime? dateTime) =>
        dateTime is null
            ? null
            : DateTime.SpecifyKind(
                dateTime.Value.Kind == DateTimeKind.Unspecified
                    ? dateTime.Value
                    : dateTime.Value.ToUniversalTime(),
                DateTimeKind.Unspecified);
}