using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
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
    /// The SQL dialect configuration used for generating database queries.
    /// </summary>
    internal static readonly ISqlDialects Dialect;

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
        new(Dialect, SchemaName, TableName);

    /// <summary>
    /// The <see cref="IdentifierTypes.ProcedureName"/> associated
    /// with <typeparamref name="T"/> if it represents a stored procedure.
    /// </summary>
    internal static readonly ProcedureName ProcedureName;

    /// <summary>
    /// Gets the <see cref="Tags.ProcedureTag"/> representing the stored procedure
    /// associated with <typeparamref name="T"/>, if applicable.
    /// </summary>
    internal static ProcedureTag ProcedureTag =>
        new(SchemaName, ProcedureName);

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
    internal static IEnumerable<ColumnInfo> ColumnInfo { get; private set; }

    /// <summary>
    /// A collection of <see cref="ReflectorCache.ColumnInfo"/> instances that exclude
    /// key columns defined on <typeparamref name="T"/>.
    /// </summary>
    internal static readonly IEnumerable<ColumnInfo> ColumnInfoLessKeys;

    /// <summary>
    /// The <see cref="ReflectorCache.ColumnInfo"/> representing the encryption key version column,
    /// or <c>null</c> if no encrypted columns are defined for <typeparamref name="T"/>.
    /// </summary>
    internal static ColumnInfo? KeyVersionColumnInfo =>
        KeyVersionColumnsInfo.FirstOrDefault();

    /// <summary>
    /// Determines whether the specified <paramref name="column"/> is marked as encrypted.
    /// </summary>
    /// <param name="column">The column to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the column is encrypted; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="column"/> is <c>null</c>.
    /// </exception>
    internal static bool IsEncrypted(ColumnInfo column)
    {
        ArgumentNullException.ThrowIfNull(column, nameof(column));
        return _EncryptedColumnInfoHashSet.Contains(column);
    }

    /// <summary>
    /// Determines whether <typeparamref name="T"/> defines any encrypted columns.
    /// </summary>
    /// <returns>
    /// <c>true</c> if one or more columns are encrypted; otherwise, <c>false</c>.
    /// </returns>
    internal static bool HasEncryptedColumns() =>
        _EncryptedColumnInfoHashSet.Count != 0;

    /// <summary>
    /// Indicates whether <typeparamref name="T"/> defines one or more columns with an alias name.
    /// </summary>
    internal static readonly bool HasAliasedColumns;

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
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="propertyNames"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="propertyNames"/> contains disallowed <c>null</c> values.
    /// </exception>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when one or more <see cref="PropertyName"/> do not match valid
    /// column properties in <typeparamref name="T"/>.
    /// </exception>
    internal static IEnumerable<ColumnInfo> GetColumnsFromProperties(params IEnumerable<PropertyName> propertyNames)
    {
        ArgumentNullException.ThrowIfNull(propertyNames, nameof(propertyNames));
        IEnumerable<PropertyName> keys = propertyNames.Materialize(NullOptionsEnum.Exception);

        InvalidPropertyException<T>? invalidPropertyException = _ColumnInfoCache.GetExceptionForInvalidProperties(keys);
        if (invalidPropertyException is not null)
            throw invalidPropertyException;
        else
            return _ColumnInfoCache.GetMany(keys);
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

        IdentifierAttribute? identifierAttribute = Type.GetCustomAttribute<IdentifierAttribute>();
        TableAttribute? tableAttribute = Type.GetCustomAttribute<TableAttribute>();

        string? schemaName =
            identifierAttribute?.Schema?.ToString().GetValueOrNull()
                ?? tableAttribute?.Schema?.GetValueOrNull();

        DialectAttribute? dialectAttribute = Type.GetCustomAttribute<DialectAttribute>();
        DialectEnum dialectEnum = dialectAttribute?.DialectEnum ?? DialectEnum.SqlServer;
        Dialect = DialectProvider.GetDialect(dialectEnum);

        SchemaName = SchemaName.New(schemaName);

        TableName = new
        (
            identifierAttribute?.Name?.ToString().GetValueOrNull()
                ?? tableAttribute?.Name?.GetValueOrNull()
                ?? Type.Name
        );

        ProcedureName = new
        (
            identifierAttribute?.Name?.ToString().GetValueOrNull()
                ?? Type.Name
        );

        HashSet<Type> supportedTypes = [.. SqlTypeCache.GetAllCSharpTypes()];

        IEnumerable<PropertyInfo> readableProperties =
            ReflectorCache<T>
                .ReadablePublicInstanceProperties
                .Materialize(NullOptionsEnum.Exception);

        IEnumerable<PropertyInfo> properties =
            readableProperties
                .Where(property =>
                    property.GetCustomAttribute<NotMappedAttribute>() is null
                    && property.PropertyType != typeof(object) // filters out SqlDbType.Variant
                    && supportedTypes.Contains(property.PropertyType))
                .Materialize(NullOptionsEnum.Exception);

        IEnumerable<PropertyInfo> primaryKeys =
            readableProperties
                .Where(property => property.GetCustomAttribute<PrimaryKeyAttribute>() is not null)
                .Materialize(NullOptionsEnum.Exception);

        IEnumerable<PropertyInfo> keys;

        if (primaryKeys.None())
            keys =
                readableProperties
                    .Where(property => property.GetCustomAttribute<KeyAttribute>() is not null)
                    .Materialize(NullOptionsEnum.Exception);
        else
            keys = primaryKeys;

        _ColumnInfoCache = new ColumnInfoCache<T, ColumnInfo>
        (
            properties.Select(property =>
                new Tuple<PropertyInfo, ColumnInfo>
                (
                    property,
                    new ColumnInfo(Dialect, SchemaName, TableName, property, keys)
                ))
        );

        ColumnInfo =
            _ColumnInfoCache
                .Values
                .Materialize(NullOptionsEnum.Exception);

        KeyColumnInfo =
            ColumnInfo
                .Where(column => column.IsKeyPart)
                .Materialize(NullOptionsEnum.Exception);

        ColumnInfoLessKeys =
            ColumnInfo
                .Where(column => column.IsKeyPart is false)
                .Materialize(NullOptionsEnum.Exception);

        HasKeyProperty = KeyColumnInfo.Any();

        KeyVersionColumnsInfo =
            ColumnInfo
                .Where(column => column.IsKeyVersionProperty)
                .Materialize(NullOptionsEnum.Exception);

        _EncryptedColumnInfoHashSet =
        [
            .. ColumnInfo.Where(column => column.IsEncrypted),
        ];

        HasAliasedColumns = ColumnInfo.Any(column => column.AliasName is not null);
    }
}
