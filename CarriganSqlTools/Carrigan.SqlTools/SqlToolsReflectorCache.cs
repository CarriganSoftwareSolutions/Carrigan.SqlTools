using Carrigan.Core.Attributes;
using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools;

/// <summary>
/// Provides lazily initialized, reflection-based metadata for the data model
/// type <typeparamref name="T"/> used by the SQL tools library.
/// </summary>
/// <typeparam name="T">
/// The entity or data model type that maps to a SQL table or stored procedure.
/// </typeparam>
public class SqlToolsReflectorCache<T>
{
    /// <summary>
    /// Gets the CLR <see cref="Type"/> that represents <typeparamref name="T"/>.
    /// </summary>
    internal static readonly Type Type;

    /// <summary>
    /// Gets all key-column <see cref="ColumnTag"/> instances for <typeparamref name="T"/>.
    /// </summary>
    internal static IEnumerable<ColumnTag> KeyColumns =>
        _LazyKeyColumns.Value;

    /// <summary>
    /// Gets all key-column <see cref="ColumnTag"/> instances for <typeparamref name="T"/>.
    /// </summary>
    internal static IEnumerable<ColumnTag> Columns =>
        _LazyColumnsDictionary.Value.Values;

    /// <summary>
    /// <summary>
    /// Gets all non-key <see cref="ColumnTag"/> instances for <typeparamref name="T"/>.
    /// </summary>
    internal static IEnumerable<ColumnTag> ColumnsLessKeys =>
        _LazyColumnsLessKeys.Value;

    /// <summary>
    /// Gets the <see cref="TableTag"/> for <typeparamref name="T"/>.
    /// </summary>
    internal static TableTag Table => 
        _LazyTableTag.Value;

    /// <summary>
    /// Gets the <see cref="ProcedureTag"/> for <typeparamref name="T"/>.
    /// </summary>
    internal static ProcedureTag ProcedureTag => 
        _LazyProcedureTag.Value;

    /// <summary>
    /// Gets the <see cref="ColumnTag"/> used to store the encryption key version,
    /// or <c>null</c> if the model has no encrypted columns.
    /// </summary>
    internal static ColumnTag? KeyVersionColumn =>
        _LazyKeyVersionColumn.Value;

    /// <summary>
    /// Determines whether the specified <paramref name="column"/> is flagged for encryption.
    /// </summary>
    /// <param name="column">The column to check.</param>
    /// <returns><c>true</c> if the column is encrypted; otherwise, <c>false</c>.</returns>
    internal static bool ContainsEncryptedProperty(ColumnTag column) =>
        _LazyEncryptedColumnsHashSet.Value.Contains(column);

    /// <summary>
    /// Determines whether <typeparamref name="T"/> defines any encrypted columns.
    /// </summary>
    /// <returns><c>true</c> if one or more columns are encrypted; otherwise, <c>false</c>.</returns>
    internal static bool HasEncryptedColumns() =>
        _LazyEncryptedColumnsHashSet.Value.Count != 0;


    /// <summary>
    /// Resolves an enumeration of <see cref="ColumnTag"/> objects for the provided property names.
    /// </summary>
    /// <param name="propertyNames">One or more property names on <typeparamref name="T"/>.</param>
    /// <returns>All matching <see cref="ColumnTag"/> instances.</returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when one or more property names do not match any qualifying
    /// column properties in <typeparamref name="T"/>.
    /// </exception>
    internal static IEnumerable<ColumnTag> GetColumnsFromProperties(params IEnumerable<string> propertyNames)
    {
        IEnumerable<string> invalidPropertyNames =
            propertyNames
                .Where(propertyName => ContainsProperty(propertyName) is false);

        IEnumerable<ColumnTag> columns =
            propertyNames
                .Where(propertyName => ContainsProperty(propertyName)) //we are testing column names here, so filter out property names with no corresponding column
                .Select(propertyName => GetColumnByProperty(propertyName))
                .OfType<ColumnTag>(); //filter nulls, clear null reference warning

        if (invalidPropertyNames.Any())
            throw new InvalidPropertyException<T>(invalidPropertyNames);
        else
            return columns; 
    }

    /// <summary>
    /// Determines whether a column mapping exists for the specified property name.
    /// </summary>
    /// <param name="propertyName">The <see cref="PropertyInfo.Name"/> to check.</param>
    /// <returns><c>true</c> if a mapping exists; otherwise, <c>false</c>.</returns>
    private static bool ContainsProperty(string propertyName) =>
        _LazyColumnsDictionary.Value.ContainsKey(propertyName);

    /// <summary>
    /// Returns the <see cref="ColumnTag"/> mapped to the specified property name,
    /// or <c>null</c> if none exists.
    /// </summary>
    /// <param name="propertyName">The <see cref="PropertyInfo.Name"/> to resolve.</param>
    /// <returns>The corresponding <see cref="ColumnTag"/>, or <c>null</c>.</returns>
    private static ColumnTag? GetColumnByProperty(string propertyName) =>
        ContainsProperty(propertyName) ? _LazyColumnsDictionary.Value[propertyName] : null;

    /// <summary>
    /// Lazily resolves the <see cref="TableTag"/> for <typeparamref name="T"/> and
    /// seeds other table-dependent caches.
    /// </summary>
    private static readonly Lazy<TableTag> _LazyTableTag;

    /// <summary>
    /// Lazily resolves the <see cref="ProcedureTag"/> for <typeparamref name="T"/>.
    /// </summary>
    private static readonly Lazy<ProcedureTag> _LazyProcedureTag;

    /// <summary>
    /// Lazily resolves the <see cref="ColumnTag"/> representing the encryption key version,
    /// if present on <typeparamref name="T"/>.
    /// </summary>
    private static readonly Lazy<ColumnTag?> _LazyKeyVersionColumn;

    /// <summary>
    /// Lazily resolves the <see cref="TableAttribute"/> (if any) on <typeparamref name="T"/>,
    /// used when deriving table name and schema.
    /// </summary>
    private static readonly Lazy<TableAttribute?> _LazyTableAttribute;

    /// <summary>
    /// Lazily resolves all public, readable, mapped properties on <typeparamref name="T"/>
    /// that are supported by the SQL generator.
    /// </summary>
    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyProperties;

    /// <summary>
    /// Lazily resolves all public, readable, mapped properties that are not part of the key
    /// on <typeparamref name="T"/>.
    /// </summary>
    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyPropertiesLessKeys;

    /// <summary>
    /// Lazily resolves all key-column <see cref="ColumnTag"/> instances for <typeparamref name="T"/>.
    /// Used to help set <see cref="ColumnsLessKeys"/>
    /// Used to help set <see cref="_LazyKeyColumns"/>
    /// Used to help set <see cref="_LazyPropertiesLessKeys"/>
    /// Used to help set <see cref="_LazyColumnsLessKeys"/>
    /// </summary>
    private static readonly Lazy<IEnumerable<ColumnTag>> _LazyKeyColumns;

    /// <summary>
    /// Lazily resolves all non-key <see cref="ColumnTag"/> instances for <typeparamref name="T"/>.
    /// Used to help set <see cref="KeyColumns"/>
    /// Used to help set <see cref="_LazyKeyVersionColumn"/>
    /// </summary>
    private static readonly Lazy<IEnumerable<ColumnTag>> _LazyColumnsLessKeys;

    /// <summary>
    /// Lazily resolves the set of encrypted column tags for <typeparamref name="T"/>.
    /// Used by <see cref="ContainsEncryptedProperty"/>
    /// Used by <see cref="HasEncryptedColumns"/>
    /// </summary>
    private static readonly Lazy<HashSet<ColumnTag>> _LazyEncryptedColumnsHashSet;

    /// <summary>
    /// Lazily resolves a dictionary mapping property names to <see cref="ColumnTag"/> instances
    /// for <typeparamref name="T"/>.
    /// Used by <see cref="Columns"/> 
    /// Used by <see cref="ContainsProperty"/>
    /// Used by <see cref="GetColumnByProperty"/>
    /// Used to help set <see cref="_LazyColumnsLessKeys"/>
    /// </summary>
    private static readonly Lazy<Dictionary<string, ColumnTag>> _LazyColumnsDictionary;

    /// <summary>
    /// Static constructor that initializes all lazy caches for <typeparamref name="T"/>.
    /// </summary>
    static SqlToolsReflectorCache()
    {
        Type = typeof(T);

        _LazyTableAttribute = new (() => Type.GetCustomAttribute<TableAttribute>());

        //Procedure Tags self validate the SQL Identifier.
        _LazyProcedureTag = new (new ProcedureTag(GetSchemaName(), GetProcedureName()));

        //Table Tags self validate the SQL Identifier.
        _LazyTableTag = new(new TableTag(GetSchemaName(), GetTableName()));

        // Get properties that are public, not marked with [NotMapped], and are either value types or strings
        _LazyProperties = new
            (() =>
                ReflectorCache<T>
                    .ReadablePublicInstanceProperties
                    .Where(property => property.GetCustomAttribute<NotMappedAttribute>() == null
                                && (property.PropertyType == typeof(int) ||              // SQL INT
                                    property.PropertyType == typeof(int?) ||             // SQL INT (nullable)
                                    property.PropertyType == typeof(long) ||             // SQL BIGINT
                                    property.PropertyType == typeof(long?) ||            // SQL BIGINT (nullable)
                                    property.PropertyType == typeof(short) ||            // SQL SMALLINT
                                    property.PropertyType == typeof(short?) ||           // SQL SMALLINT (nullable)
                                    property.PropertyType == typeof(byte) ||             // SQL TINYINT
                                    property.PropertyType == typeof(byte?) ||            // SQL TINYINT (nullable)
                                    property.PropertyType == typeof(bool) ||             // SQL BIT
                                    property.PropertyType == typeof(bool?) ||            // SQL BIT (nullable)
                                    property.PropertyType == typeof(decimal) ||          // SQL DECIMAL
                                    property.PropertyType == typeof(decimal?) ||         // SQL DECIMAL (nullable)
                                    property.PropertyType == typeof(float) ||            // SQL FLOAT
                                    property.PropertyType == typeof(float?) ||           // SQL FLOAT (nullable)
                                    property.PropertyType == typeof(double) ||           // SQL REAL
                                    property.PropertyType == typeof(double?) ||          // SQL REAL (nullable)
                                    property.PropertyType == typeof(string) ||           // SQL NVARCHAR, VARCHAR, TEXT
                                    property.PropertyType == typeof(DateTime) ||         // SQL DATETIME, DATETIME2, SMALLDATETIME
                                    property.PropertyType == typeof(DateTime?) ||        // SQL DATETIME, DATETIME2, SMALLDATETIME (nullable)
                                    property.PropertyType == typeof(Guid) ||             // SQL UNIQUEIDENTIFIER
                                    property.PropertyType == typeof(Guid?) ||            // SQL UNIQUEIDENTIFIER (nullable)
                                    property.PropertyType == typeof(byte[]) ||           // SQL VARBINARY
                                    property.PropertyType == typeof(char) ||             // SQL CHAR
                                    property.PropertyType == typeof(char?) ||            // SQL CHAR (nullable)
                                    property.PropertyType == typeof(TimeOnly) ||         // SQL Time
                                    property.PropertyType == typeof(TimeOnly?) ||        // SQL Time (nullable)
                                    property.PropertyType == typeof(DateOnly) ||         // SQL Date
                                    property.PropertyType == typeof(DateOnly?) ||        // SQL Date (nullable)
                                    property.PropertyType == typeof(DateTimeOffset) ||   // SQL DateTimeOffset
                                    property.PropertyType == typeof(DateTimeOffset?))    // SQL DateTimeOffset (nullable)
                        )
            );

        _LazyKeyColumns = new
            (() =>
                {
                    IEnumerable<PropertyInfo> keys =
                        ReflectorCache<T>
                            .ReadablePublicInstanceProperties
                            .Where(property => property.GetCustomAttribute<PrimaryKeyAttribute>() != null);
                    if(keys.None()) 
                        keys =
                            ReflectorCache<T>
                                .ReadablePublicInstanceProperties
                                .Where(property => property.GetCustomAttribute<KeyAttribute>() != null);

                    return keys
                        .Select(property => new ColumnTag(_LazyTableTag.Value, GetColumnName(property), property, GetParameterName(property)));
                }
            );

        _LazyPropertiesLessKeys = new 
            (() =>
                {
                    IEnumerable<PropertyInfo> keyProperties = 
                        _LazyKeyColumns
                            .Value
                            .Select(column => column._propertyInfo);
                    return _LazyProperties
                        .Value
                        .Where(property => keyProperties.DoesNotContain(property));
                }
            );

        _LazyEncryptedColumnsHashSet = new
            (() =>
                [.. _LazyPropertiesLessKeys
                        .Value
                        .Where(property => property.GetCustomAttribute<EncryptedAttribute>() != null)
                        .Select(property => new ColumnTag(_LazyTableTag.Value, GetColumnName(property), property, GetParameterName(property)))]
            );

        _LazyColumnsDictionary = new Lazy<Dictionary<string, ColumnTag>>
            (() =>
                [.. _LazyProperties
                    .Value
                    .Select(property => new KeyValuePair<string, ColumnTag>(property.Name, new ColumnTag(_LazyTableTag.Value, GetColumnName(property), property, GetParameterName(property))))]
            );

        _LazyColumnsLessKeys = new
            (() =>
                _LazyColumnsDictionary
                    .Value
                    .Values
                    .Where(column => _LazyKeyColumns.Value.DoesNotContain(column))
            );

        _LazyKeyVersionColumn = new
            (() => _LazyProperties
                            .Value
                            .Where(property => property.GetCustomAttribute<KeyVersionAttribute>() != null)
                            .Where(property => ((Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType) == typeof(int)))
                            .Select(property => new ColumnTag(_LazyTableTag.Value, GetColumnName(property), property, GetParameterName(property)))
                            .FirstOrDefault()
            );
    }

    /// <summary>
    /// Resolves the SQL column name to use for the given <see cref="PropertyInfo"/>.
    /// Checks custom identifiers first, then <see cref="ColumnAttribute"/>, then falls back
    /// to the property name; validates against SQL identifier rules.
    /// </summary>
    /// <param name="property">The property to resolve.</param>
    /// <returns>The validated column name.</returns>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when the resolved name fails SQL identifier validation.
    /// </exception>
    private static string GetColumnName(PropertyInfo property)
    {
        IdentifierAttribute? identifier = property.GetCustomAttribute<IdentifierAttribute>();
        if (identifier != null && identifier.Name.IsNotNullOrWhiteSpace())
            if (SqlIdentifierPattern.Fails(identifier.Name))
                throw new InvalidSqlIdentifierException(identifier.Name);
            else
                return identifier.Name;
        else
        {
            ColumnAttribute? columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            if(columnAttribute != null && columnAttribute.Name.IsNotNullOrWhiteSpace())
                if (SqlIdentifierPattern.Fails(columnAttribute.Name))
                    throw new InvalidSqlIdentifierException(columnAttribute.Name);
                else
                    return columnAttribute.Name;
            else  if (SqlIdentifierPattern.Fails(property.Name))
                throw new InvalidSqlIdentifierException(property.Name);
            else
                return property.Name;
        }
    }

    /// <summary>
    /// Resolves the SQL parameter <see cref="ParameterTag"/> for the given <see cref="PropertyInfo"/>.
    /// Uses <see cref="ParameterAttribute"/> when present; otherwise derives from the column name.
    /// </summary>
    /// <param name="property">The property to resolve.</param>
    /// <returns>The <see cref="ParameterTag"/> to use for SQL generation.</returns>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when an attribute-specified name fails SQL identifier validation.
    /// </exception>
    private static ParameterTag GetParameterName(PropertyInfo property)
    {
        ParameterAttribute? parameterName = property.GetCustomAttribute<ParameterAttribute>();

        if (parameterName != null && parameterName.Name.IsNotNullOrWhiteSpace())
        {
            if (SqlIdentifierPattern.Fails(parameterName.Name))
                throw new InvalidSqlIdentifierException(parameterName.Name);
            else
                return new(null, parameterName.Name, null);
        }
        else
        {
            return new(null, GetColumnName(property), null);
        }
    }

    /// <summary>
    /// Resolves the SQL table name for <typeparamref name="T"/>. Checks custom identifiers first,
    /// then <see cref="TableAttribute"/>, then falls back to the CLR type name; validates the result.
    /// </summary>
    /// <returns>The validated table name.</returns>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when the resolved name fails SQL identifier validation.
    /// </exception>
    private static string GetTableName()
    {
        IdentifierAttribute? identifier = Type.GetCustomAttribute<IdentifierAttribute>();
        if (identifier != null && identifier.Name.IsNotNullOrWhiteSpace())
            if (SqlIdentifierPattern.Fails(identifier.Name))
                throw new InvalidSqlIdentifierException(identifier.Name);
            else
                return identifier.Name;
        else
        {
            if (_LazyTableAttribute.Value != null && _LazyTableAttribute.Value.Name.IsNotNullOrWhiteSpace())
                if (SqlIdentifierPattern.Fails(_LazyTableAttribute.Value.Name))
                    throw new InvalidSqlIdentifierException(_LazyTableAttribute.Value.Name);
                else
                    return _LazyTableAttribute.Value.Name;
            else if (SqlIdentifierPattern.Fails(Type.Name))
                throw new InvalidSqlIdentifierException(Type.Name);
            else
                return Type.Name;
        }
    }

    /// <summary>
    /// Resolves the SQL stored procedure name for <typeparamref name="T"/>.
    /// Checks custom identifiers first, then falls back to the CLR type name; validates the result.
    /// </summary>
    /// <returns>The validated procedure name.</returns>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when the resolved name fails SQL identifier validation.
    /// </exception>
    private static string GetProcedureName()
    {
        IdentifierAttribute? identifier = Type.GetCustomAttribute<IdentifierAttribute>();
        if (identifier != null && identifier.Name.IsNotNullOrWhiteSpace())
            if (SqlIdentifierPattern.Fails(identifier.Name))
                throw new InvalidSqlIdentifierException(identifier.Name);
            else
                return identifier.Name;
        else if (SqlIdentifierPattern.Fails(Type.Name))
                throw new InvalidSqlIdentifierException(Type.Name);
        else
            return Type.Name;
    }

    /// <summary>
    /// Resolves the SQL schema name for <typeparamref name="T"/>. Checks custom identifiers first,
    /// then <see cref="TableAttribute"/>, otherwise returns an empty string; validates when present.
    /// </summary>
    /// <returns>The schema name, or <see cref="string.Empty"/> if none.</returns>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when a provided schema name fails SQL identifier validation.
    /// </exception>
    private static string GetSchemaName()
    {
        IdentifierAttribute? identifier = Type.GetCustomAttribute<IdentifierAttribute>();
        if (identifier != null && identifier.Schema.IsNotNullOrWhiteSpace())
            if (SqlIdentifierPattern.Fails(identifier.Schema))
                throw new InvalidSqlIdentifierException(identifier.Schema);
            else
                return identifier.Schema;
        else
        {
            if (_LazyTableAttribute.Value != null && _LazyTableAttribute.Value.Schema.IsNotNullOrWhiteSpace())
                if (SqlIdentifierPattern.Fails(_LazyTableAttribute.Value.Schema))
                    throw new InvalidSqlIdentifierException(_LazyTableAttribute.Value.Schema);
                else
                    return _LazyTableAttribute.Value.Schema;
            else
                return string.Empty;
        }
    }
}
