using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Dialects;

/// <summary>
/// Provides SQL dialect-specific formatting and rendering logic for PostgreSQL.
/// </summary>
public class PostgreSqlDialect : ISqlDialects
{
    /// <summary>
    /// Encloses the specified identifier in PostgreSQL double quotes.
    /// Embedded double quotes are escaped by doubling them.
    /// </summary>
    /// <param name="identifier">The identifier value.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
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
    /// <param name="schemaName">The SQL schema name to apply.</param>
    /// <param name="tableName">The SQL table name to apply.</param>
    public string RenderTable(SchemaName? schemaName, TableName tableName) =>
        schemaName.IsNotNullOrEmpty()
            ? $"{QuoteIdentifier(schemaName)}.{QuoteIdentifier(tableName)}"
            : QuoteIdentifier(tableName);

    /// <summary>
    /// Renders the fully qualified PostgreSQL column name.
    /// </summary>
    /// <param name="tableTag">The tableTag value.</param>
    /// <param name="columnName">The SQL column name to apply.</param>
    /// <param name="includeTable">The includeTable value.</param>
    public string RenderColumn(TableTag tableTag, ColumnName columnName, bool includeTable = true) =>
        includeTable && tableTag.ToString().IsNotNullOrEmpty()
            ? $"{tableTag.ToSql(this)}.{QuoteIdentifier(columnName)}"
            : QuoteIdentifier(columnName);

    /// <summary>
    /// Generates SQL fragments for an INSERT statement that returns inserted values
    /// by appending PostgreSQL's RETURNING clause.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="insertIntoFragments">The insertIntoFragments value.</param>
    /// <param name="insertValuesFragments">The insertValuesFragments value.</param>
    /// <param name="columnInfo">The reflected column metadata for the model property.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
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

    /// <summary>
    /// Builds the PostgreSQL RETURNING clause for inserted columns.
    /// </summary>
    /// <typeparam name="T">The model type used to resolve result column names.</typeparam>
    /// <param name="columnInfo">The columns to include in the RETURNING clause.</param>
    /// <returns>A RETURNING clause that projects the inserted columns.</returns>
    private string ReturningColumns<T>(IEnumerable<ColumnInfo> columnInfo) =>
        $"RETURNING {string.Join(", ", columnInfo.Select(ReturningColumn<T>))}";

    /// <summary>
    /// Builds a PostgreSQL RETURNING projection for a single inserted column.
    /// </summary>
    /// <typeparam name="T">The model type used to resolve result column names.</typeparam>
    /// <param name="columnInfo">The inserted column to project.</param>
    /// <returns>The quoted column name, including an alias when the result column name differs.</returns>
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
    /// <param name="paging">The paging fragment to include in the query.</param>
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
    /// <param name="baseParameterName">The baseParameterName value.</param>
    /// <param name="parameterIndex">The parameterIndex value.</param>
    public string RenderFinalParameterName(string baseParameterName, int parameterIndex) =>
        $"${parameterIndex}";

    /// <summary>
    /// Returns the default PostgreSQL field properties for a CLR type.
    /// </summary>
    /// <param name="type">The type value.</param>
    public FieldProperties GetDefaultFieldPropertiesByClrType(Type type) =>
        PostgreSqlTypesProvider.FromClrType(type);

    /// <summary>
    /// Generates the PostgreSQL declaration for a field based on the provided field properties.
    /// </summary>
    /// <param name="fieldProperties">The SQL field properties that describe the value type.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
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

        if (fieldProperties.IsArray is not null && fieldProperties.IsArray.Value)
            declaration += "[]";

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
    /// <param name="value">The parameter value.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when an argument is invalid for the requested SQL operation.
    /// </exception>
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
        else if (value is Array array && value is not byte[])
        {
            return ArrayValueConversion(array);
        }
        else
        {
            return value;
        }
    }

    /// <summary>
    /// Performs value conversions for array types to ensure compatibility with PostgreSQL's handling of array parameters.
    /// </summary>
    /// <param name="array">
    /// The array value to convert.
    /// </param>
    /// <returns>
    /// A converted array object with element types transformed as necessary for PostgreSQL compatibility.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided array does not have a valid element type for conversion.
    /// </exception>
    private object ArrayValueConversion(Array array)
    {
        Type elementType = array.GetType().GetElementType()
            ?? throw new ArgumentException("Array values must have an element type.", nameof(array));

        if (elementType == typeof(XDocument))
            return ((XDocument?[])array).Select(value => value?.ToString()).ToArray();
        if (elementType == typeof(XmlDocument))
            return ((XmlDocument?[])array).Select(value => value?.OuterXml).ToArray();

        if (elementType == typeof(char))
            return ((char[])array).Select(value => value.ToString()).ToArray();
        if (elementType == typeof(char?))
            return ((char?[])array).Select(value => value?.ToString()).ToArray();

        if (elementType == typeof(byte?))
            return ((byte?[])array).Select(value => value is null ? null : (short?)value.Value).ToArray();
        if (elementType == typeof(sbyte))
            return ((sbyte[])array).Select(value => (short)value).ToArray();
        if (elementType == typeof(sbyte?))
            return ((sbyte?[])array).Select(value => value is null ? null : (short?)value.Value).ToArray();
        if (elementType == typeof(ushort))
            return ((ushort[])array).Select(value => (int)value).ToArray();
        if (elementType == typeof(ushort?))
            return ((ushort?[])array).Select(value => value is null ? null : (int?)value.Value).ToArray();
        if (elementType == typeof(uint))
            return ((uint[])array).Select(value => (long)value).ToArray();
        if (elementType == typeof(uint?))
            return ((uint?[])array).Select(value => value is null ? null : (long?)value.Value).ToArray();
        if (elementType == typeof(ulong))
            return ((ulong[])array).Select(value => (decimal)value).ToArray();
        if (elementType == typeof(ulong?))
            return ((ulong?[])array).Select(value => value is null ? null : (decimal?)value.Value).ToArray();

        if (elementType == typeof(DateTime))
            return ((DateTime[])array).Select(value => NormalizeTimeZone(value)!.Value).ToArray();
        if (elementType == typeof(DateTime?))
            return ((DateTime?[])array).Select(NormalizeTimeZone).ToArray();
        if (elementType == typeof(DateTimeOffset))
            return ((DateTimeOffset[])array).Select(value => NormalizeTimeZone(value)!.Value).ToArray();
        if (elementType == typeof(DateTimeOffset?))
            return ((DateTimeOffset?[])array).Select(NormalizeTimeZone).ToArray();

        return array;
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
    /// Normalizes a <see cref="DateTime"/> to Unspecified, by removing the Kind.
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to normalize. If null, the method returns null.
    /// </param>
    /// <returns>
    /// A normalized <see cref="DateTime"/> value with Kind set to Unspecified, by removing the Kind.
    /// </returns>
    public DateTime? NormalizeTimeZone(DateTime? dateTime) =>
        dateTime is null
            ? null
            : DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Unspecified);
    /// <summary>
    /// Returns a set of CLR types that are supported by the PostgreSQL dialect for SQL parameter mapping and type inference.
    /// </summary>
    /// <returns>
    /// A <see cref="HashSet{Type}"/> containing the CLR types supported by the PostgreSQL dialect, including but not limited to:
    /// <list type="bullet">
    ///     <item><description><see cref="Guid"/> and nullable <see cref="Guid"/> values, including array variants.</description></item>
    ///     <item><description>Text values, including <see cref="string"/>, <see cref="char"/>, nullable <see cref="char"/>, and supported array variants.</description></item>
    ///     <item><description>Binary values, including <see cref="byte"/> arrays mapped as PostgreSQL <c>bytea</c>.</description></item>
    ///     <item><description><see cref="bool"/> and nullable <see cref="bool"/> values, including array variants.</description></item>
    ///     <item><description>Integer values, including <see cref="byte"/>, <see cref="sbyte"/>, <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, and their nullable and array variants.</description></item>
    ///     <item><description>Numeric values, including <see cref="float"/>, <see cref="double"/>, <see cref="decimal"/>, and their nullable and array variants.</description></item>
    ///     <item><description>Date and time values, including <see cref="DateTime"/>, <see cref="DateOnly"/>, <see cref="TimeOnly"/>, <see cref="TimeSpan"/>, <see cref="DateTimeOffset"/>, and their nullable and array variants.</description></item>
    ///     <item><description>XML values, including <see cref="System.Xml.Linq.XDocument"/>, <see cref="System.Xml.XmlDocument"/>, and supported array variants.</description></item>
    ///     <item><description><see cref="object"/> as a fallback type for unmapped values.</description></item>
    /// </list>
    /// </returns>
    public HashSet<Type> SupportedTypes() =>
    [
        // Guid
        typeof(Guid),
        typeof(Guid?),
        typeof(Guid[]),
        typeof(Guid?[]),

        // Text
        typeof(string),
        typeof(string[]),
        typeof(char),
        typeof(char?),
        typeof(char[]),
        typeof(char?[]),

        // Binary
        // byte[] is bytea, not a PostgreSQL array type.
        typeof(byte[]),
        typeof(byte[][]),

        // Boolean
        typeof(bool),
        typeof(bool?),
        typeof(bool[]),
        typeof(bool?[]),

        // Integers
        typeof(byte),
        typeof(byte?),
        typeof(sbyte),
        typeof(sbyte?),
        typeof(short),
        typeof(short?),
        typeof(ushort),
        typeof(ushort?),
        typeof(int),
        typeof(int?),
        typeof(uint),
        typeof(uint?),
        typeof(long),
        typeof(long?),
        typeof(ulong),
        typeof(ulong?),

        typeof(byte?[]),
        typeof(sbyte[]),
        typeof(sbyte?[]),
        typeof(short[]),
        typeof(short?[]),
        typeof(ushort[]),
        typeof(ushort?[]),
        typeof(int[]),
        typeof(int?[]),
        typeof(uint[]),
        typeof(uint?[]),
        typeof(long[]),
        typeof(long?[]),
        typeof(ulong[]),
        typeof(ulong?[]),

        // Decimal and floating-point
        typeof(float),
        typeof(float?),
        typeof(double),
        typeof(double?),
        typeof(decimal),
        typeof(decimal?),

        typeof(float[]),
        typeof(float?[]),
        typeof(double[]),
        typeof(double?[]),
        typeof(decimal[]),
        typeof(decimal?[]),

        // Date/Time
        typeof(DateTime),
        typeof(DateTime?),
        typeof(DateOnly),
        typeof(DateOnly?),
        typeof(TimeOnly),
        typeof(TimeOnly?),
        typeof(TimeSpan),
        typeof(TimeSpan?),
        typeof(DateTimeOffset),
        typeof(DateTimeOffset?),

        typeof(DateTime[]),
        typeof(DateTime?[]),
        typeof(DateOnly[]),
        typeof(DateOnly?[]),
        typeof(TimeOnly[]),
        typeof(TimeOnly?[]),
        typeof(TimeSpan[]),
        typeof(TimeSpan?[]),
        typeof(DateTimeOffset[]),
        typeof(DateTimeOffset?[]),

        // XML
        // PostgreSQL/Npgsql treats XML as string-compatible.
        typeof(System.Xml.Linq.XDocument),
        typeof(System.Xml.XmlDocument),
        typeof(System.Xml.Linq.XDocument[]),
        typeof(System.Xml.XmlDocument[]),

        // Fallback
        typeof(object),
        typeof(object[])
    ];
}
