using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Sets;
/// <summary>
/// Represents the SQL <c>SET</c> clause used when generating <c>UPDATE</c> statements.
/// Use this to specify only certain columns to update instead of updating all non-key columns.
/// The name mirrors SQL syntax: <c>SET [Column] = @Parameter</c>.
/// </summary>
/// <typeparam name="T">
/// The entity/data model type that maps to the target table.
/// </typeparam>
/// <para>Update example not using SetColumns</para>
/// <example>
/// <code language="csharp"><![CDATA[
/// Customer entity = new()
/// {
///     Id = 42,
///     Name = "Hank",
///     Email = "Hank@tx.gov",
///     Phone = "+1(555)555-5555"
/// };
/// SqlQuery query = customerGenerator.UpdateById(entity);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// UPDATE [Customer] 
/// SET [Name] = @Name, [Email] = @Email, [Phone] = @Phone 
/// WHERE [Id] = @Id;
/// ]]></code>
/// </example>
/// <example>
/// <para>
/// Update example using SetColumns
/// Note: <see cref="SetColumns{T}"/> validates the names of the properties, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// SetColumns<Customer> columns = new(nameof(Customer.Email));
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
public class SetColumns<T>
{
    /// <summary>
    /// The <see cref="ColumnInfo"/> entries corresponding to the columns included in the <c>SET</c> clause.
    /// </summary>
    internal IEnumerable<ColumnInfo> ColumnInfo { get; private set; }

    /// <summary>
    /// Initializes a new <see cref="SetColumns{T}"/> containing the specified properties (columns).
    /// </summary>
    /// <param name="propertyNames">
    /// One or more property names that map to column names to be updated.
    /// Each name is validated via the reflection cache; invalid names throw.
    /// </param>
    public SetColumns(params IEnumerable<PropertyName> propertyNames) =>
        ColumnInfo = SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyNames);

    /// <summary>
    /// Initializes a new <see cref="SetColumns{T}"/> containing the specified properties (columns).
    /// </summary>
    /// <param name="propertyNames">
    /// One or more property names that map to column names to be updated.
    /// Each name is validated via the reflection cache; invalid names throw.
    /// </param>
    [ExternalOnly]
    public SetColumns(params IEnumerable<string> propertyNames) :
        this(propertyNames.Select(name => new PropertyName(name)))
    { }

    /// <summary>
    /// Adds an additional column to the <c>SET</c> clause.
    /// </summary>
    /// <param name="propertyName">The property name mapping to the column to add.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown if <paramref name="propertyName"/> does not exist on <typeparamref name="T"/> or is ineligible.
    /// </exception>
    public void AddColumn(PropertyName propertyName)
    {
        ColumnInfo? newTag = SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyName).Single();
        if (newTag is not null)
            ColumnInfo = ColumnInfo.Append(newTag);
    }

    /// <summary>
    /// Adds an additional column to the <c>SET</c> clause.
    /// </summary>
    /// <param name="propertyName">The property name mapping to the column to add.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown if <paramref name="propertyName"/> does not exist on <typeparamref name="T"/> or is ineligible.
    /// </exception>
    [ExternalOnly]
    public void AddColumn(string propertyName) =>
        AddColumn(new PropertyName(propertyName));


    /// <summary>
    /// Creates a new instance of SetColumns&lt;T&gt; with an additional column to the <c>SET</c> clause.
    /// </summary>
    /// <param name="propertyName">The property name mapping to the column to append.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown if <paramref name="propertyName"/> does not exist on <typeparamref name="T"/> or is ineligible.
    /// </exception>
    public SetColumns<T> AppendColumn(PropertyName propertyName)
    {
        SetColumns<T> returnValue = new(ColumnInfo.Select(columnInfo => columnInfo.PropertyName));
        ColumnInfo? newTag = SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyName).Single();
        if (newTag is not null)
            returnValue.ColumnInfo = returnValue.ColumnInfo.Append(newTag);
        return returnValue;
    }

    /// <summary>
    /// Creates a new instance of SetColumns&lt;T&gt; with an additional column to the <c>SET</c> clause.
    /// </summary>
    /// <param name="propertyName">The property name mapping to the column to append.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown if <paramref name="propertyName"/> does not exist on <typeparamref name="T"/> or is ineligible.
    /// </exception>
    [ExternalOnly]
    public SetColumns<T> AppendColumn(string propertyName) =>
        AppendColumn(new PropertyName(propertyName));


    /// <summary>
    /// Creates a new instance of SetColumns&lt;T&gt; with an additional columns to the <c>SET</c> clause.
    /// </summary>
    /// <param name="propertyName">The property name mappings to the column to concat.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown if <paramref name="propertyName"/> does not exist on <typeparamref name="T"/> or is ineligible.
    /// </exception>
    public SetColumns<T> ConcatColumn(params IEnumerable<PropertyName> propertyNames)
    {
        SetColumns<T> returnValue = new(ColumnInfo.Select(columnInfo => columnInfo.PropertyName));
        IEnumerable<ColumnInfo> newTags = SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyNames);
        if (newTags is not null)
            returnValue.ColumnInfo = returnValue.ColumnInfo.Concat(newTags);
        return returnValue;
    }

    /// <summary>
    /// Creates a new instance of SetColumns&lt;T&gt; with an additional columns to the <c>SET</c> clause.
    /// </summary>
    /// <param name="propertyName">The property name mappings to the column to concat.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown if <paramref name="propertyName"/> does not exist on <typeparamref name="T"/> or is ineligible.
    /// </exception>
    [ExternalOnly]
    public SetColumns<T> ConcatColumn(params IEnumerable<string> propertyNames) =>
        ConcatColumn(propertyNames.Select(aString => new PropertyName(aString)));
}
