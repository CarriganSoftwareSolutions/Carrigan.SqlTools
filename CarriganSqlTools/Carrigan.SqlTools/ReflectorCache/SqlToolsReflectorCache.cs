using Carrigan.Core.Attributes;
using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.RegularExpressions;
using Carrigan.SqlTools.Tags;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

/// <summary>
/// Provides lazily initialized, reflection-based metadata for the data model
/// type <typeparamref name="T"/> used by the SQL tools library.
/// </summary>
/// <typeparam name="T">
/// The entity or data model type that maps to a SQL table or stored procedure.
/// </typeparam>
//TODO: Unit Tests
public class SqlToolsReflectorCache<T>
{
    /// <summary>
    /// Gets the CLR <see cref="Type"/> that represents <typeparamref name="T"/>.
    /// </summary>
    internal static readonly Type Type;

    //TODO: proof read documentation
    /// <summary>
    /// Represents the Schema name for the Class's corresponding Table and Columns
    /// </summary>
    internal static readonly SchemaName? SchemaName;
    //TODO: proof read documentation
    /// <summary>
    /// Represents the Table name for the Class's corresponding Table and Columns
    /// </summary>
    internal static readonly TableName TableName;

    /// <summary>
    /// Gets all key-column <see cref="ColumnInfo"/> instances for <typeparamref name="T"/>.
    /// </summary>
    internal static IEnumerable<ColumnInfo> KeyColumnInfo =>
        _LazyKeyColumnInfo.Value;

    /// <summary>
    /// Gets all column <see cref="ColumnInfo"/> instances for <typeparamref name="T"/>.
    /// </summary>
    internal static IEnumerable<ColumnInfo> ColumnInfo =>
        _LazyColumnInfoCache.Value.Values;

    /// <summary>
    /// <summary>
    /// Gets all non-key <see cref="ColumnInfo"/> instances for <typeparamref name="T"/>.
    /// </summary>
    internal static IEnumerable<ColumnInfo> ColumnInfoLessKeys =>
        ColumnInfo.Where(column => column.IsKeyPart is false); //TODO: cache? Probably b est.

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
    /// Gets the <see cref="ColumnInfo"/> used to store the encryption key version,
    /// or <c>null</c> if the model has no encrypted columns.
    /// </summary>
    internal static ColumnInfo? KeyVersionColumnInfo =>
        _LazyKeyVersionColumnInfo.Value.FirstOrDefault();

    /// <summary>
    /// Determines whether the specified <paramref name="column"/> is flagged for encryption.
    /// </summary>
    /// <param name="column">The column to check.</param>
    /// <returns><c>true</c> if the column is encrypted; otherwise, <c>false</c>.</returns>
    internal static bool IsEncrypted(ColumnInfo column) =>
        _LazyEncryptedColumnInfoHashSet.Value.Contains(column);

    /// <summary>
    /// Determines whether <typeparamref name="T"/> defines any encrypted columns.
    /// </summary>
    /// <returns><c>true</c> if one or more columns are encrypted; otherwise, <c>false</c>.</returns>
    internal static bool HasEncryptedColumns() =>
        _LazyEncryptedColumnInfoHashSet.Value.Count != 0;

    /// <summary>
    /// Resolves an enumeration of <see cref="ColumnInfo"/> objects for the provided property names.
    /// </summary>
    /// <param name="propertyNames">One or more property names on <typeparamref name="T"/>.</param>
    /// <returns>All matching <see cref="ColumnInfo"/> instances.</returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when one or more property names do not match any qualifying
    /// column properties in <typeparamref name="T"/>.
    /// </exception>
    internal static IEnumerable<ColumnInfo> GetColumnsFromProperties(params IEnumerable<PropertyName> propertyNames)
    {
        InvalidPropertyException<T>? invalidPropertyException = _LazyColumnInfoCache.Value.GetExceptionForInvalidProperties(propertyNames);
        if (invalidPropertyException is not null)
            throw invalidPropertyException;
        else
            return _LazyColumnInfoCache.Value.GetMany(propertyNames);
    }

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
    /// </summary>;
    protected static readonly Lazy<IEnumerable<ColumnInfo>> _LazyKeyVersionColumnInfo;

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

    //TODO: proof read documentation.
    /// <summary>
    /// Lazily resolves all public, readable, mapped properties on <typeparamref name="T"/>
    /// that are supported by the SQL generator and designated as part of the Key.
    /// </summary>
    private static readonly Lazy<IEnumerable<ColumnInfo>> _LazyKeyColumnInfo;

    //TODO: proof read documentation.
    /// <summary>
    /// Lazily a hash for Encrypted ColumnInfo
    /// </summary>
    private static readonly Lazy<HashSet<ColumnInfo>> _LazyEncryptedColumnInfoHashSet;

    /// <summary>
    /// TODO: Proof Read Documentation
    /// Lazily resolves a PropertyInfoCache for <see cref="ColumnInfo"/> instances
    /// for <typeparamref name="T"/>.
    /// </summary>
    private static readonly Lazy<PropertyInfoCache<T, ColumnInfo>> _LazyColumnInfoCache;

    /// <summary>
    /// Static constructor that initializes all lazy caches for <typeparamref name="T"/>.
    /// </summary>
    /// 
    static SqlToolsReflectorCache()
    {
        Type = typeof(T);

        _LazyTableAttribute = new (() => Type.GetCustomAttribute<TableAttribute>());

        SchemaName = GetSchemaName();
        TableName = GetTableName();

        // Get properties that are public, not marked with [NotMapped], and are either value types or strings
        _LazyProperties = new
        (() =>
            ReflectorCache<T>
                .ReadablePublicInstanceProperties
                .Where
                (   property => property.GetCustomAttribute<NotMappedAttribute>() == null
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

        //Procedure Tags self validate the SQL Identifier.
        _LazyProcedureTag = new(new ProcedureTag(SchemaName, GetProcedureName()));

        //Table Tags self validate the SQL Identifier.
        _LazyTableTag = new(new TableTag(SchemaName, TableName));

        _LazyColumnInfoCache = new (() =>
        {
            IEnumerable<PropertyInfo> keys =
                ReflectorCache<T>
                    .ReadablePublicInstanceProperties
                    .Where(property => property.GetCustomAttribute<PrimaryKeyAttribute>() != null);
            if (keys.None())
                keys =
                    ReflectorCache<T>
                        .ReadablePublicInstanceProperties
                        .Where(property => property.GetCustomAttribute<KeyAttribute>() != null);

            return new PropertyInfoCache<T, ColumnInfo>
            (
                _LazyProperties.Value.Select
                (
                    property =>
                    new Tuple<PropertyInfo, ColumnInfo>
                    (
                        property,
                        new ColumnInfo(SchemaName, TableName, property, keys)
                    )
                )
            );
        });

        _LazyKeyColumnInfo = new
        (() =>
            _LazyColumnInfoCache
                .Value
                .Values
                .Where(column => column.IsKeyPart)
        );

        _LazyKeyVersionColumnInfo = new
        (() =>
            _LazyColumnInfoCache
                .Value
                .Values
                .Where(column => column.IsKeyVersionField)
        );

        _LazyEncryptedColumnInfoHashSet = new
        (() =>
            [.._LazyColumnInfoCache
                .Value
                .Values
                .Where(column => column.IsEncrypted)]
        );
    }

    /// <summary>
    /// Resolves the SQL table name for <typeparamref name="T"/>. Checks custom identifiers first,
    /// then <see cref="TableAttribute"/>, then falls back to the CLR type name; validates the result.
    /// </summary>
    /// <returns>The validated table name.</returns>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when the resolved name fails SQL identifier validation.
    /// </exception>
    private static TableName GetTableName()
    {
        IdentifierAttribute? identifier = Type.GetCustomAttribute<IdentifierAttribute>();
        if (identifier != null && identifier.Name.IsNotNullOrWhiteSpace())
            if (SqlIdentifierPattern.Fails(identifier.Name))
                throw new InvalidSqlIdentifierException(identifier.Name);
            else
                return new TableName(identifier.Name);
        else
        {
            if (_LazyTableAttribute.Value != null && _LazyTableAttribute.Value.Name.IsNotNullOrWhiteSpace())
                if (SqlIdentifierPattern.Fails(_LazyTableAttribute.Value.Name))
                    throw new InvalidSqlIdentifierException(_LazyTableAttribute.Value.Name);
                else
                    return new TableName(_LazyTableAttribute.Value.Name);
            else if (SqlIdentifierPattern.Fails(Type.Name))
                throw new InvalidSqlIdentifierException(Type.Name);
            else
                return new TableName(Type.Name);
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
    private static SchemaName? GetSchemaName()
    {
        IdentifierAttribute? identifier = Type.GetCustomAttribute<IdentifierAttribute>();
        if (identifier != null && identifier.Schema.IsNotNullOrWhiteSpace())
            if (SqlIdentifierPattern.Fails(identifier.Schema))
                throw new InvalidSqlIdentifierException(identifier.Schema);
            else
                return new (identifier.Schema);
        else
        {
            if (_LazyTableAttribute.Value != null && _LazyTableAttribute.Value.Schema.IsNotNullOrWhiteSpace())
                if (SqlIdentifierPattern.Fails(_LazyTableAttribute.Value.Schema))
                    throw new InvalidSqlIdentifierException(_LazyTableAttribute.Value.Schema);
                else
                    return new (_LazyTableAttribute.Value.Schema);
            else
                return null;
        }
    }
}
