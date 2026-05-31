using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.RegularExpressions;
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
        new(SchemaName, TableName);

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
    /// <param name="supportedTypes">
    /// A set of supported types to filter the columns by.
    /// </param>
    internal static IEnumerable<ColumnInfo> GetColumnInfo(HashSet<Type> supportedTypes) =>
        _ColumnInfoCache.Values.Where(columnInfo => supportedTypes.Contains(columnInfo.Type));

    /// <summary>
    /// A collection of <see cref="ReflectorCache.ColumnInfo"/> instances that exclude
    /// key columns defined on <typeparamref name="T"/>.
    /// </summary>
    /// <param name="supportedTypes">
    /// A set of supported types to filter the columns by.
    /// </param>
    internal static IEnumerable<ColumnInfo> GetGetColumnInfoLessKeys(HashSet<Type> supportedTypes) =>
        _ColumnInfoCache.Values.Where(columnInfo => supportedTypes.Contains(columnInfo.Type) && KeyColumnInfo.DoesNotContain(columnInfo));

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
    /// <param name="supportedTypes">
    /// A set of supported types to filter the columns by.
    /// </param>
    internal static bool HasAliasedColumns(HashSet<Type> supportedTypes) =>
        GetColumnInfo(supportedTypes)
                .Any(column => column.AliasName is not null);

    /// <summary>
    /// Creates select tags for every supported mapped column on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="selectTagT">The concrete select-tag type to create.</typeparam>
    /// <param name="supportedTypes">A set of supported types to filter the columns by.</param>
    /// <param name="selectTagFactory">The factory used to create the concrete select-tag type.</param>
    /// <returns>All supported select tags for <typeparamref name="T"/>.</returns>
    internal static IEnumerable<selectTagT> CreateAllSelectTags<selectTagT>
    (
        HashSet<Type> supportedTypes,
        Func<ColumnTag, AliasTag?, selectTagT> selectTagFactory
    )
        where selectTagT : SelectTagBase =>
            GetColumnInfo(supportedTypes)
                .Select(column => CreateSelectTag(column, null, selectTagFactory));

    /// <summary>
    /// Resolves an enumeration of <see cref="ReflectorCache.ColumnInfo"/> instances that correspond
    /// to the provided <see cref="PropertyName"/>.
    /// </summary>
    /// <param name="supportedTypes"></param>
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
    /// <param name="propertyNames">
    /// One or more <see cref="PropertyName"/> defined on <typeparamref name="T"/>.
    /// </param>
    internal static IEnumerable<ColumnInfo> GetColumnsFromProperties(HashSet<Type> supportedTypes, params IEnumerable<PropertyName> propertyNames)
    {
        ArgumentNullException.ThrowIfNull(propertyNames, nameof(propertyNames));
        IEnumerable<PropertyName> keys = propertyNames.Materialize(NullOptionsEnum.Exception);

        InvalidPropertyException<T>? invalidPropertyException = _ColumnInfoCache.GetExceptionForInvalidProperties(supportedTypes, keys);
        if (invalidPropertyException is not null)
            throw invalidPropertyException;
        else
            return _ColumnInfoCache.GetMany(keys);
    }

    /// <summary>
    /// Resolves the <see cref="ReflectorCache.ColumnInfo"/> instance that corresponds
    /// to the provided <see cref="PropertyName"/>.
    /// </summary>
    /// <param name="supportedTypes">
    /// A set of supported types to filter the columns by.
    /// </param>
    /// <param name="propertyNames">
    /// The <see cref="PropertyName"/> for which to resolve column information.
    /// </param>
    /// <returns>
    /// The matching <see cref="ReflectorCache.ColumnInfo"/> object.
    /// </returns>
    internal static ColumnInfo GetColumnsFromProperty(HashSet<Type> supportedTypes, PropertyName propertyNames)
    {
        ArgumentNullException.ThrowIfNull(propertyNames, nameof(propertyNames));

        InvalidPropertyException<T>? invalidPropertyException = _ColumnInfoCache.GetExceptionForInvalidProperties(supportedTypes, propertyNames);
        if (invalidPropertyException is not null)
            throw invalidPropertyException;
        else
            return _ColumnInfoCache.Get(propertyNames);
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
    private static readonly ColumnInfoCache<T> _ColumnInfoCache;

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

        IEnumerable<PropertyInfo> readableProperties =
            ReflectorCache<T>
                .ReadablePublicInstanceProperties
                .Materialize(NullOptionsEnum.Exception);

        IEnumerable<PropertyInfo> properties =
            readableProperties
                .Where(property =>
                    property.GetCustomAttribute<NotMappedAttribute>() is null
                    && property.PropertyType != typeof(object)) // filters out SqlDbType.Variant
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

        _ColumnInfoCache = new ColumnInfoCache<T>
        (
            properties.Select(property =>
                new Tuple<PropertyInfo, ColumnInfo>
                (
                    property,
                    new ColumnInfo(SchemaName, TableName, property, keys)
                ))
        );

        //TODO: ensure each SqlGenerator validates the columns are supported.
        KeyColumnInfo =
            _ColumnInfoCache
                .Values
                .Where(column => column.IsKeyPart)
                .Materialize(NullOptionsEnum.Exception);

        HasKeyProperty = KeyColumnInfo.Any();

        //TODO: ensure each SqlGenerator validates the columns are supported.
        KeyVersionColumnsInfo =
            _ColumnInfoCache
                .Values
                .Where(column => column.IsKeyVersionProperty)
                .Materialize(NullOptionsEnum.Exception);

        _EncryptedColumnInfoHashSet =
        [
            .. _ColumnInfoCache
                .Values
                .Where(column => column.IsEncrypted),
        ];
    }


    /// <summary>
    /// Creates a concrete select tag for the specified property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="selectTagT">The concrete select-tag type to create.</typeparam>
    /// <param name="propertyName">The name of the property to project.</param>
    /// <param name="supportedTypes">A set of supported types to filter the columns by.</param>
    /// <param name="selectTagFactory">The factory used to create the concrete select-tag type.</param>
    /// <param name="aliasName">An optional alias name override.</param>
    /// <returns>A concrete select tag representing the requested property projection.</returns>
    internal static selectTagT CreateSelectTag<selectTagT>
    (
        PropertyName propertyName,
        HashSet<Type> supportedTypes,
        Func<ColumnTag, AliasTag?, selectTagT> selectTagFactory,
        AliasName? aliasName = null
    )
        where selectTagT : SelectTagBase
    {
        ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));
        ArgumentNullException.ThrowIfNull(supportedTypes, nameof(supportedTypes));
        ArgumentNullException.ThrowIfNull(selectTagFactory, nameof(selectTagFactory));

        ColumnInfo columnInfo = GetColumnsFromProperty(supportedTypes, propertyName);
        return CreateSelectTag(columnInfo, aliasName, selectTagFactory);
    }

    /// <summary>
    /// Creates concrete select tags for the specified properties on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="selectTagT">The concrete select-tag type to create.</typeparam>
    /// <param name="supportedTypes">A set of supported types to filter the columns by.</param>
    /// <param name="selectTagFactory">The factory used to create the concrete select-tag type.</param>
    /// <param name="propertyNames">The property names to project.</param>
    /// <returns>The concrete select tags representing the requested property projections.</returns>
    internal static IEnumerable<selectTagT> CreateSelectTags<selectTagT>
    (
        HashSet<Type> supportedTypes,
        Func<ColumnTag, AliasTag?, selectTagT> selectTagFactory,
        params IEnumerable<PropertyName> propertyNames
    )
        where selectTagT : SelectTagBase
    {
        ArgumentNullException.ThrowIfNull(supportedTypes, nameof(supportedTypes));
        ArgumentNullException.ThrowIfNull(selectTagFactory, nameof(selectTagFactory));
        ArgumentNullException.ThrowIfNull(propertyNames, nameof(propertyNames));

        return GetColumnsFromProperties(supportedTypes, propertyNames)
            .Select(columnInfo => CreateSelectTag(columnInfo, null, selectTagFactory));
    }

    /// <summary>
    /// Creates a neutral reflected select tag for the specified property without filtering by dialect support.
    /// </summary>
    /// <remarks>
    /// This overload exists for attributes that are attached directly to a property and therefore need to
    /// describe projection metadata before a dialect-specific generator is involved.
    /// </remarks>
    internal static SelectTagBase GetSelectTag(PropertyName propertyName, AliasName? aliasName = null)
    {
        ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

        ColumnInfo columnInfo = _ColumnInfoCache.Get(propertyName);
        return CreateSelectTag(columnInfo, aliasName, static (columnTag, aliasTag) => new ReflectedSelectTag(columnTag, aliasTag));
    }

    private static selectTagT CreateSelectTag<selectTagT>
    (
        ColumnInfo columnInfo,
        AliasName? aliasName,
        Func<ColumnTag, AliasTag?, selectTagT> selectTagFactory
    )
        where selectTagT : SelectTagBase
    {
        ArgumentNullException.ThrowIfNull(columnInfo, nameof(columnInfo));
        ArgumentNullException.ThrowIfNull(selectTagFactory, nameof(selectTagFactory));

        if (aliasName.IsNotNullOrEmpty() && SqlIdentifierPattern.Fails(aliasName))
            throw new InvalidSqlIdentifierException(aliasName);

        ColumnTag columnTag = columnInfo.SelectTag.ColumnTag;
        AliasTag? aliasTag = aliasName is null
            ? columnInfo.SelectTag.AliasTag
            : AliasTag.New(aliasName);

        return selectTagFactory(columnTag, aliasTag);
    }

}
