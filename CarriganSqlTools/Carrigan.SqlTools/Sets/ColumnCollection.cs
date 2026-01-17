using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.Sets;

/// <summary>
/// Represents a validated collection of model properties (SQL columns) used to define
/// which columns are included in generated SQL statements (for example: UPDATE SET,
/// INSERT column lists, and INSERT return/output column lists).
/// </summary>
/// <typeparam name="T">The entity or data model type used to resolve SQL column metadata.</typeparam>
/// <remarks>
/// This type validates CLR property names against <typeparamref name="T"/> using
/// <see cref="SqlToolsReflectorCache{T}"/> and stores the resolved <see cref="ReflectorCache.ColumnInfo"/>
/// metadata.
/// <para>
/// Duplicate property names are ignored, preserving the first occurrence.
/// </para>
/// </remarks>
/// <example>
/// <para>
/// Update example using ColumnCollection
/// Note: <see cref="ColumnCollection{T}"/> validates the names of the properties, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnCollection<Customer> columns = new(nameof(Customer.Email));
/// Customer entity = new()
/// {
///     Id = 42,
///     Name = "Hank",
///     Email = "Hank@example.gov"
/// };
/// SqlQuery query = customerGenerator.UpdateById(entity, columns);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// UPDATE [Customer] 
/// SET [Email] = @Email 
/// WHERE [Id] = @Id;
/// ]]></code>
/// </example>
public class ColumnCollection<T>
{
    /// <summary>
    /// Gets the resolved <see cref="ColumnInfo"/> metadata for this collection.
    /// </summary>
    /// <remarks>
    /// This enumerable is always materialized (not lazy) to avoid repeated reflection/cache
    /// look ups and to prevent deep iterator chains from repeated Append/Concat operations.
    /// </remarks>
    internal IEnumerable<ColumnInfo> ColumnInfo { get; private set; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnCollection{T}"/> class.
    /// </summary>
    /// <param name="propertyNames">The property names to include in the collection.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyNames"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="propertyNames"/> contains a disallowed <c>null</c> entry.
    /// </exception>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when one or more property names do not exist on <typeparamref name="T"/>.
    /// </exception>
    public ColumnCollection(params IEnumerable<PropertyName> propertyNames) => 
        ColumnInfo =
            SqlToolsReflectorCache<T>
                .GetColumnsFromProperties(propertyNames)
                .DistinctBy(static columnInfo => columnInfo.PropertyName)
                .Materialize(NullOptionsEnum.Exception);

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnCollection{T}"/> class.
    /// </summary>
    /// <param name="propertyNames">The CLR property name strings to include in the collection.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyNames"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when one or more property names do not exist on <typeparamref name="T"/>.
    /// </exception>
    [ExternalOnly]
    public ColumnCollection(params IEnumerable<string> propertyNames) :
        this(propertyNames.Select(name => new PropertyName(name)))
    { }

    /// <summary>
    /// Adds a column to the current collection.
    /// </summary>
    /// <param name="propertyName">The property name to add.</param>
    /// <remarks>
    /// If the property already exists in the collection, this method does not add a duplicate.
    /// </remarks>
    /// <exception cref="NullReferenceException">Thrown when <paramref name="propertyName"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not exist on <typeparamref name="T"/>.
    /// </exception>
    public void AddColumn(PropertyName propertyName)
    {
        bool alreadyIncluded =
            ColumnInfo
                .Select(static columnInfo => columnInfo.PropertyName)
                .Contains(propertyName);

        if (alreadyIncluded)
        {
            return;
        }

        ColumnInfo resolvedColumnInfo =
            SqlToolsReflectorCache<T>
                .GetColumnsFromProperties(propertyName)
                .Single();

        ColumnInfo =
            ColumnInfo
                .Append(resolvedColumnInfo)
                .Materialize(NullOptionsEnum.Exception);
    }

    /// <summary>
    /// Adds a column to the current collection.
    /// </summary>
    /// <param name="propertyName">The property name to add.</param>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not exist on <typeparamref name="T"/>.
    /// </exception>
    [ExternalOnly]
    public void AddColumn(string propertyName) =>
        AddColumn(new PropertyName(propertyName));

    /// <summary>
    /// Returns a new <see cref="ColumnCollection{T}"/> with an additional column.
    /// </summary>
    /// <param name="propertyName">The property name to append.</param>
    /// <returns>A new collection that includes the appended column.</returns>
    /// <remarks>
    /// If the property already exists in the collection, the returned collection is unchanged.
    /// </remarks>
    /// <exception cref="NullReferenceException">Thrown when <paramref name="propertyName"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not exist on <typeparamref name="T"/>.
    /// </exception>
    public ColumnCollection<T> AppendColumn(PropertyName propertyName)
    {
        ColumnCollection<T> returnValue =
            new(ColumnInfo.Select(static columnInfo => columnInfo.PropertyName));

        bool alreadyIncluded =
            returnValue
                .ColumnInfo
                .Select(static columnInfo => columnInfo.PropertyName)
                .Contains(propertyName);

        if (alreadyIncluded)
        {
            return returnValue;
        }

        ColumnInfo resolvedColumnInfo =
            SqlToolsReflectorCache<T>
                .GetColumnsFromProperties(propertyName)
                .Single();

        returnValue.ColumnInfo =
            returnValue
                .ColumnInfo
                .Append(resolvedColumnInfo)
                .Materialize(NullOptionsEnum.Exception);

        return returnValue;
    }

    /// <summary>
    /// Returns a new <see cref="ColumnCollection{T}"/> with an additional column.
    /// </summary>
    /// <param name="propertyName">The property name to append.</param>
    /// <returns>A new collection that includes the appended column.</returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not exist on <typeparamref name="T"/>.
    /// </exception>
    [ExternalOnly]
    public ColumnCollection<T> AppendColumn(string propertyName) =>
        AppendColumn(new PropertyName(propertyName));

    /// <summary>
    /// Returns a new <see cref="ColumnCollection{T}"/> with additional columns appended.
    /// </summary>
    /// <param name="propertyNames">The property names to append.</param>
    /// <returns>A new collection that includes the appended columns.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyNames"/> is <c>null</c>.</exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="propertyNames"/> contains a disallowed <c>null</c> entry.
    /// </exception>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when any name in <paramref name="propertyNames"/> does not exist on <typeparamref name="T"/>.
    /// </exception>
    public ColumnCollection<T> ConcatColumn(params IEnumerable<PropertyName> propertyNames)
    {
        ColumnCollection<T> returnValue =
            new(ColumnInfo.Select(static columnInfo => columnInfo.PropertyName));

        IEnumerable<ColumnInfo> additionalColumnInfo =
            SqlToolsReflectorCache<T>
                .GetColumnsFromProperties(propertyNames);

        returnValue.ColumnInfo =
            returnValue
                .ColumnInfo
                .Concat(additionalColumnInfo)
                .DistinctBy(static columnInfo => columnInfo.PropertyName)
                .Materialize(NullOptionsEnum.Exception);

        return returnValue;
    }

    /// <summary>
    /// Returns a new <see cref="ColumnCollection{T}"/> with additional columns appended.
    /// </summary>
    /// <param name="propertyNames">The property names to append.</param>
    /// <returns>A new collection that includes the appended columns.</returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when any name in <paramref name="propertyNames"/> does not exist on <typeparamref name="T"/>.
    /// </exception>
    [ExternalOnly]
    public ColumnCollection<T> ConcatColumn(params IEnumerable<string> propertyNames) =>
        ConcatColumn(propertyNames.Select(static propertyName => new PropertyName(propertyName)));
}
