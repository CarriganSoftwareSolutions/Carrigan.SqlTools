using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a model property (i.e., a table column) as a leaf node within predicate logic
/// for SQL <c>WHERE</c> and <c>JOIN</c> clauses.
/// </summary>
/// <typeparam name="T">
/// The entity or data model type that defines the table containing the referenced column.
/// </typeparam>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterName = new("Name", "Hank");
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// Equal equalName = new(columnName, parameterName);
/// SqlQuery query = customerGenerator.Select(null, null, equalName, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
public class Column<T> : ColumnBase
{
    /// <summary>
    /// The name of the property representing the column.
    /// </summary>
    internal PropertyName PropertyName { get; }

    /// <summary>
    /// Creates a standardized <see cref="ArgumentException"/> for an invalid <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">The property name that failed validation.</param>
    /// <returns>
    /// An <see cref="ArgumentException"/> describing the invalid property and the corresponding model/table context.
    /// </returns>
    internal static ArgumentException NoSuchProperty(PropertyName propertyName) =>
        new($"{propertyName} is not a valid property name on {SqlToolsReflectorCache<T>.Type.Name}, representing: {SqlToolsReflectorCache<T>.Table}.", nameof(propertyName));

    /// <summary>
    /// Initializes a new <see cref="Column{T}"/> using a property name.
    /// </summary>
    /// <param name="propertyName">The property name that identifies the column.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="propertyName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid, eligible property on <typeparamref name="T"/>.
    /// </exception>
    [ExternalOnly]
    public Column(string propertyName) : this(new PropertyName(propertyName))
    { }

    /// <summary>
    /// Initializes a new <see cref="Column{T}"/> using a <see cref="PropertyName"/> wrapper.
    /// </summary>
    /// <param name="propertyName">The property name wrapper that identifies the column.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="propertyName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid, eligible property on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown only if the property passes validation but no matching column metadata is returned.
    /// This is not expected under normal conditions.
    /// </exception>
    public Column(PropertyName propertyName) : base(GetColumnInfo(propertyName)) =>
        PropertyName = propertyName;

    private static ColumnInfo GetColumnInfo(PropertyName propertyName)
    {
        ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

        return SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyName).SingleOrDefault()
            ?? throw NoSuchProperty(propertyName);
    }

    /// <summary>
    /// Produces the SQL fragment represented by this column.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated through the predicate tree to disambiguate duplicate parameter names.
    /// Not used by columns.
    /// </param>
    /// <param name="branchName">
    /// The branch prefix that is prepended to the beginning of all parameter names in this predicate tree.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied parameter names that are duplicated and may require a prefix.
    /// Not used by columns.
    /// </param>
    /// <returns>
    /// The SQL-escaped column identifier (e.g., <c>[Schema].[Table].[Column]</c> or <c>[Table].[Column]</c>).
    /// </returns>
    internal override IEnumerable<SqlFragment> ToSqlFragments()
    {
        yield return new SqlFragmentText(ColumnInfo);
    }
}
