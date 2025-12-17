using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

/// <summary>
/// Provides a cached reflection-based lookup for metadata describing
/// how the data model type <typeparamref name="T"/> maps to SQL identifiers
/// such as schema, table, procedure, and column names.
/// <para>
/// This cache forms the foundation of the SQL generation system,
/// allowing column and parameter metadata to be resolved efficiently
/// from <see cref="PropertyInfo"/> and related attributes.
/// </para>
/// </summary>
/// <typeparam name="T">
/// The entity or data model type that represents a SQL table or stored procedure.
/// </typeparam>

public class SqlToolsReflectorCache<T>
{
    /// <summary>
    /// Gets the CLR <see cref="Type"/> that represents <typeparamref name="T"/>.
    /// </summary>
    internal static readonly Type Type;

    /// <summary>
    /// The <see cref="IdentifierTypes.SchemaName"/> associated with
    /// the SQL schema for <typeparamref name="T"/> and its columns.
    /// </summary>
    internal static readonly SchemaName? SchemaName;

    /// <summary>
    /// The <see cref="IdentifierTypes.TableName"/> that identifies
    /// the SQL table corresponding to <typeparamref name="T"/>.
    /// </summary>
    internal static readonly TableName TableName;

    /// <summary>
    /// Gets the <see cref="TableTag"/> that represents
    /// the fully qualified table identifier for <typeparamref name="T"/>.
    /// </summary>
    internal static TableTag Table =>
        new (SchemaName, TableName);


    /// <summary>
    /// Gets the <see cref="Tags.ProcedureTag"/> representing the stored procedure
    /// associated with <typeparamref name="T"/>, if applicable.
    /// </summary>
    internal static ProcedureTag ProcedureTag =>
        new (SchemaName, ProcedureName);

    /// <summary>
    /// A collection of <see cref="ReflectorCache.ColumnInfo"/> instances representing
    /// the key columns defined on <typeparamref name="T"/>.
    /// </summary>
    internal static readonly IEnumerable<ColumnInfo> KeyColumnInfo;

    /// <summary>
    /// Indicates whether <typeparamref name="T"/> defines
    /// at least one key property.
    /// </summary>
    internal static readonly bool HasKeyProperty;

    /// <summary>
    /// Gets all <see cref="ReflectorCache.ColumnInfo"/> instances defined for <typeparamref name="T"/>.
    /// </summary>
    internal static IEnumerable<ColumnInfo> ColumnInfo =>
        _ColumnInfoCache.Values;

    /// <summary>
    /// A collection of <see cref="ReflectorCache.ColumnInfo"/> instances that exclude
    /// key columns defined on <typeparamref name="T"/>.
    /// </summary>
    internal static readonly IEnumerable<ColumnInfo> ColumnInfoLessKeys;

    /// <summary>
    /// The <see cref="IdentifierTypes.ProcedureName"/> associated
    /// with <typeparamref name="T"/> if it represents a stored procedure.
    /// </summary>
    internal readonly static ProcedureName ProcedureName;
    /// <summary>
    /// The <see cref="ReflectorCache.ColumnInfo"/> representing the encryption key version column,
    /// or <c>null</c> if no encrypted columns are defined for <typeparamref name="T"/>.
    /// </summary>
    /// <summary>
    internal static ColumnInfo? KeyVersionColumnInfo =>
        KeyVersionColumnsInfo.FirstOrDefault();

    /// <summary>
    /// Determines whether the specified <paramref name="column"/> is marked as encrypted.
    /// </summary>
    /// <param name="column">The column to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the column is encrypted; otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsEncrypted(ColumnInfo column) =>
        _EncryptedColumnInfoHashSet.Contains(column);


    /// <summary>
    /// Determines whether <typeparamref name="T"/> defines any encrypted columns.
    /// </summary>
    /// <returns>
    /// <c>true</c> if one or more columns are encrypted; otherwise, <c>false</c>.
    /// </returns>
    internal static bool HasEncryptedColumns() =>
        _EncryptedColumnInfoHashSet.Count != 0;

    //TODO: unit test
    /// <summary>
    /// Indicates whether <typeparamref name="T"/> defines one or more columns with an alias name.
    /// </summary>
    /// <returns>
    /// <c>true</c> if at least one column has an alias name; otherwise, <c>false</c>.
    /// </returns>
    internal static readonly bool HasAliasedColumns;

    //TODO: unit test
    /// <summary>
    /// Gets a <see cref="Tags.SelectTags"/> collection representing the <see cref="SelectTag"/>
    /// for each mapped column on <typeparamref name="T"/>.
    /// </summary>
    internal static SelectTags SelectTags =>
        new(ColumnInfo.Select(column => column.SelectTag));

    /// <summary>
    /// Resolves an enumeration of <see cref="ReflectorCache.ColumnInfo"/> instances that correspond
    /// to the provided <see cref="PropertyName"/>.
    /// </summary>
    /// <param name="propertyNames">
    /// One or more <see cref="PropertyName"/> defined on <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// The matching <see cref="ReflectorCache.ColumnInfo"/> objects.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when one or more <see cref="PropertyName"/> do not match valid
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
    /// A collection of <see cref="ReflectorCache.ColumnInfo"/> objects representing
    /// encryption key version columns, if present on <typeparamref name="T"/>.
    /// </summary>
    protected static readonly IEnumerable<ColumnInfo> KeyVersionColumnsInfo;

    /// <summary>
    /// A hash set containing all <see cref="ReflectorCache.ColumnInfo"/> instances
    /// marked as encrypted for <typeparamref name="T"/>.
    /// </summary>
    private static readonly HashSet<ColumnInfo> _EncryptedColumnInfoHashSet;

    /// <summary>
    /// A cached lookup of <see cref="PropertyInfo"/> objects mapped to
    /// corresponding <see cref="ReflectorCache.ColumnInfo"/> metadata for <typeparamref name="T"/>.
    /// </summary>
    private static readonly ColumnInfoCache<T, ColumnInfo> _ColumnInfoCache;

    /// <summary>
    /// Static constructor that initializes the reflection-based metadata cache
    /// for the data model type <typeparamref name="T"/>.
    /// </summary>
    static SqlToolsReflectorCache()
    {
        Type = typeof(T);

        string? schemaName =
            Type.GetCustomAttribute<IdentifierAttribute>()?.Schema?.ToString().GetValueOrNull()
                ?? Type.GetCustomAttribute<TableAttribute>()?.Schema?.GetValueOrNull();
        SchemaName = SchemaName.New(schemaName);

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
                        && property.PropertyType != typeof(object) //to filter out properties with types that map to SqlDbType.Variant
                        && SqlTypeCache.GetAllCSharpTypes().Contains(property.PropertyType)
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

        _ColumnInfoCache = new ColumnInfoCache<T, ColumnInfo>
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

        ColumnInfoLessKeys =
            ColumnInfo.Where(column => column.IsKeyPart is false); 

        HasKeyProperty = KeyColumnInfo.Any();

        KeyVersionColumnsInfo =
            _ColumnInfoCache
                .Values
                .Where(column => column.IsKeyVersionProperty);

        _EncryptedColumnInfoHashSet =
            [.._ColumnInfoCache
                .Values
                .Where(column => column.IsEncrypted)];

        HasAliasedColumns = ColumnInfo.Any(column => column.AliasName is not null);
    }
}
