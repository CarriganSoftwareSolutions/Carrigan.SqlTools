using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Dialects;

/// <summary>
/// Provides dialect-neutral rendering used for diagnostic string representations.
/// </summary>
/// <remarks>
/// This dialect intentionally leaves identifiers and parameter names unquoted and unfinalized so expression
/// <see cref="object.ToString()"/> implementations can use the same SQL-fragment pipeline as database-specific rendering.
/// </remarks>
internal sealed class NeutralDialect : ISqlDialects
{
    /// <summary>
    /// Gets the shared neutral dialect instance used for diagnostic rendering.
    /// </summary>
    internal static NeutralDialect Instance { get; } = new();

    /// <summary>
    /// Returns an identifier without applying database-specific quoting.
    /// </summary>
    public string QuoteIdentifier(string identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier);
        return identifier;
    }

    /// <summary>
    /// Renders a procedure tag without database-specific quoting.
    /// </summary>
    public string RenderProcedureTag(ProcedureTag procedure)
    {
        ArgumentNullException.ThrowIfNull(procedure);
        return procedure.ToString();
    }

    /// <summary>
    /// Renders a table name and optional schema without database-specific quoting.
    /// </summary>
    public string RenderTable(SchemaName? schemaName, TableName tableName)
    {
        ArgumentNullException.ThrowIfNull(tableName);
        return schemaName is not null && schemaName.IsNotEmpty() ? $"{schemaName}.{tableName}" : tableName.ToString();
    }

    /// <summary>
    /// Renders a column and optional table qualification without database-specific quoting.
    /// </summary>
    public string RenderColumn(TableTag tableTag, ColumnName columnName, bool includeTable = true)
    {
        ArgumentNullException.ThrowIfNull(tableTag);
        ArgumentNullException.ThrowIfNull(columnName);
        return includeTable && tableTag.IsNotEmpty() ? $"{tableTag}.{columnName}" : columnName.ToString();
    }

    /// <summary>
    /// Returns the supplied INSERT fragments unchanged for neutral diagnostic rendering.
    /// </summary>
    public IEnumerable<ISqlFragment> GetInsertReturningFragments<T>
    (
        IEnumerable<ISqlFragment> insertIntoFragments,
        IEnumerable<ISqlFragment> insertValuesFragments,
        IEnumerable<ColumnInfo> columnInfo
    )
    {
        ArgumentNullException.ThrowIfNull(insertIntoFragments);
        ArgumentNullException.ThrowIfNull(insertValuesFragments);
        ArgumentNullException.ThrowIfNull(columnInfo);
        return insertIntoFragments.Concat(insertValuesFragments);
    }

    /// <summary>
    /// Leaves a parameter name unchanged so diagnostic rendering does not add dialect delimiters or positional indexes.
    /// </summary>
    public string RenderFinalParameterName(string baseParameterName, int parameterIndex)
    {
        ArgumentNullException.ThrowIfNull(baseParameterName);
        return baseParameterName;
    }

    /// <summary>
    /// Renders a neutral OFFSET/FETCH representation.
    /// </summary>
    public ISqlFragment RenderPaging(PagingBase paging)
    {
        ArgumentNullException.ThrowIfNull(paging);

        return new SqlFragmentText
        (
            (paging.Offset, paging.Next) switch
            {
                (0u, 0u) => string.Empty,
                (_, 0u) => $"OFFSET {paging.Offset} ROWS",
                (0u, _) => $"FETCH NEXT {paging.Next} ROWS ONLY",
                _ => $"OFFSET {paging.Offset} ROWS FETCH NEXT {paging.Next} ROWS ONLY"
            }
        );
    }

    /// <summary>
    /// Returns neutral field properties based on the CLR type name.
    /// </summary>
    public FieldProperties GetDefaultFieldPropertiesByClrType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return new() { BaseType = type.Name, ProviderTypeName = type.Name };
    }

    /// <summary>
    /// Renders the neutral type name represented by the field properties.
    /// </summary>
    public string RenderFieldProperties(FieldProperties fieldProperties) =>
        RenderCastType(fieldProperties);

    /// <summary>
    /// Returns parameter values unchanged, except that null is represented by <see cref="DBNull.Value"/>.
    /// </summary>
    public object ValueConversion(object? value) =>
        value ?? DBNull.Value;

    /// <summary>
    /// Returns the dialect-neutral XOR token.
    /// </summary>
    public ISqlFragment GetXOrSymbol() =>
        new SqlFragmentText("XOR");

    /// <summary>
    /// Returns the diagnostic LIKE token for the requested case-sensitivity behavior.
    /// </summary>
    public ISqlFragment GetDialectLike(bool? isCaseSensitive = null) =>
        new SqlFragmentText
        (
            isCaseSensitive switch
            {
                null => "LIKE",
                true => "CASE SENSITIVE LIKE",
                false => "CASE INSENSITIVE LIKE"
            }
        );

    /// <summary>
    /// Indicates that neutral rendering permits fully qualified SET expressions.
    /// </summary>
    public bool DoesUpdateSupportsFullyQualifiedSets() =>
        true;

    /// <summary>
    /// Returns a <see cref="DateTimeOffset"/> unchanged.
    /// </summary>
    public DateTimeOffset? NormalizeTimeZone(DateTimeOffset? dateTimeOffset) =>
        dateTimeOffset;

    /// <summary>
    /// Returns a <see cref="DateTime"/> unchanged.
    /// </summary>
    public DateTime? NormalizeTimeZone(DateTime? dateTime) =>
        dateTime;

    /// <summary>
    /// Returns an empty type set because neutral diagnostic rendering does not restrict supported CLR types.
    /// </summary>
    public HashSet<Type> SupportedTypes() =>
        [];

    /// <summary>
    /// Creates neutral field properties from the runtime CLR value.
    /// </summary>
    public FieldProperties FromClrValue(object? value)
    {
        Type? type = value?.GetType();
        return type is null ? new() : GetDefaultFieldPropertiesByClrType(type);
    }

    /// <summary>
    /// Returns the neutral cast type represented by the supplied field properties.
    /// </summary>
    public string RenderCastType(FieldProperties fieldProperties)
    {
        ArgumentNullException.ThrowIfNull(fieldProperties);
        return fieldProperties.BaseType ?? string.Empty;
    }
}
