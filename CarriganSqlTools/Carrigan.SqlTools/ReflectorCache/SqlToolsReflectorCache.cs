using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
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
    /// Gets the <see cref="TableTag"/> for <typeparamref name="T"/>.
    /// </summary>
    internal static TableTag Table =>
        new (SchemaName, TableName);

    /// <summary>
    /// Gets the <see cref="ProcedureTag"/> for <typeparamref name="T"/>.
    /// </summary>
    internal static ProcedureTag ProcedureTag =>
        new (SchemaName, ProcedureName);

    /// <summary>
    /// Gets all key-column <see cref="ColumnInfo"/> instances for <typeparamref name="T"/>.
    /// </summary>
    internal static readonly IEnumerable<ColumnInfo> KeyColumnInfo;

    /// <summary>
    /// Gets all column <see cref="ColumnInfo"/> instances for <typeparamref name="T"/>.
    /// </summary>
    internal static IEnumerable<ColumnInfo> ColumnInfo =>
        _ColumnInfoCache.Values;

    /// <summary>
    /// <summary>
    /// Gets all non-key <see cref="ColumnInfo"/> instances for <typeparamref name="T"/>.
    /// </summary>
    internal static IEnumerable<ColumnInfo> ColumnInfoLessKeys =>
        ColumnInfo.Where(column => column.IsKeyPart is false); //TODO: cache? Probably b est.

    /// <summary>
    /// Gets the <see cref="ProcedureName"/> for <typeparamref name="T"/>.
    /// </summary>
    internal readonly static ProcedureName ProcedureName;

    /// <summary>
    /// Gets the <see cref="ColumnInfo"/> used to store the encryption key version,
    /// or <c>null</c> if the model has no encrypted columns.
    /// </summary>
    internal static ColumnInfo? KeyVersionColumnInfo =>
        KeyVersionColumnsInfo.FirstOrDefault();

    /// <summary>
    /// Determines whether the specified <paramref name="column"/> is flagged for encryption.
    /// </summary>
    /// <param name="column">The column to check.</param>
    /// <returns><c>true</c> if the column is encrypted; otherwise, <c>false</c>.</returns>
    internal static bool IsEncrypted(ColumnInfo column) =>
        _EncryptedColumnInfoHashSet.Contains(column);

    /// <summary>
    /// Determines whether <typeparamref name="T"/> defines any encrypted columns.
    /// </summary>
    /// <returns><c>true</c> if one or more columns are encrypted; otherwise, <c>false</c>.</returns>
    internal static bool HasEncryptedColumns() =>
        _EncryptedColumnInfoHashSet.Count != 0;

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
        InvalidPropertyException<T>? invalidPropertyException = _ColumnInfoCache.GetExceptionForInvalidProperties(propertyNames);
        if (invalidPropertyException is not null)
            throw invalidPropertyException;
        else
            return _ColumnInfoCache.GetMany(propertyNames);
    }

    /// <summary>
    /// Lazily resolves the <see cref="ColumnTag"/> representing the encryption key version,
    /// if present on <typeparamref name="T"/>.
    /// </summary>;
    protected static readonly IEnumerable<ColumnInfo> KeyVersionColumnsInfo;

    //TODO: proof read documentation.
    /// <summary>
    /// Lazily a hash for Encrypted ColumnInfo
    /// </summary>
    private static readonly HashSet<ColumnInfo> _EncryptedColumnInfoHashSet;

    /// <summary>
    /// TODO: Proof Read Documentation
    /// Lazily resolves a PropertyInfoCache for <see cref="ColumnInfo"/> instances
    /// for <typeparamref name="T"/>.
    /// </summary>
    private static readonly PropertyInfoCache<T, ColumnInfo> _ColumnInfoCache;

    /// <summary>
    /// Static constructor that initializes all lazy caches for <typeparamref name="T"/>.
    /// </summary>
    static SqlToolsReflectorCache()
    {
        Type = typeof(T);

        string? schemaName =
            Type.GetCustomAttribute<IdentifierAttribute>()?.Schema?.ToString().GetValueOrNull()
                ?? Type.GetCustomAttribute<TableAttribute>()?.Schema?.GetValueOrNull();
        SchemaName = schemaName is not null ? new(schemaName) : null;

        TableName = new 
        (
            Type.GetCustomAttribute<IdentifierAttribute>()?.Name?.ToString().GetValueOrNull()
                ?? Type.GetCustomAttribute<TableAttribute>()?.Name?.GetValueOrNull()
                ?? Type.Name
        );

        ProcedureName = new
        (
            Type.GetCustomAttribute<IdentifierAttribute>()?.Name?.ToString().GetValueOrNull()
                ?? Type.Name
        );

        // Get properties that are public, not marked with [NotMapped], and are either value types or strings
        IEnumerable<PropertyInfo> properties =
            ReflectorCache<T>
                .ReadablePublicInstanceProperties
                .Where
                (property => property.GetCustomAttribute<NotMappedAttribute>() == null
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
                );

        IEnumerable<PropertyInfo> keys =
            ReflectorCache<T>
                .ReadablePublicInstanceProperties
                .Where(property => property.GetCustomAttribute<PrimaryKeyAttribute>() != null);
        if (keys.None())
            keys =
                ReflectorCache<T>
                    .ReadablePublicInstanceProperties
                    .Where(property => property.GetCustomAttribute<KeyAttribute>() != null);

        _ColumnInfoCache = new PropertyInfoCache<T, ColumnInfo>
        (
            properties.Select
            (
                property =>
                new Tuple<PropertyInfo, ColumnInfo>
                (
                    property,
                    new ColumnInfo(SchemaName, TableName, property, keys)
                )
            )
        );

        KeyColumnInfo =
            _ColumnInfoCache
                .Values
                .Where(column => column.IsKeyPart);

        KeyVersionColumnsInfo =
            _ColumnInfoCache
                .Values
                .Where(column => column.IsKeyVersionField);

        _EncryptedColumnInfoHashSet =
            [.._ColumnInfoCache
                .Values
                .Where(column => column.IsEncrypted)];
    }
}
