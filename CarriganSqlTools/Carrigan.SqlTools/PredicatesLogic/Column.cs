using Carrigan.SqlTools.Attributes;
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
///  <see cref="Column{T}"/> validates property names and throws an exception if a property name is invalid.
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
    /// The name of the property representing the column
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
        new ($"{propertyName} is not the valid name of a property in the class, {SqlToolsReflectorCache<T>.Type.Name}, representing: {SqlToolsReflectorCache<T>.Table}.", nameof(propertyName));

    /// <summary>
    /// Initializes a new <see cref="Column{T}"/> using a property name.
    /// </summary>
    /// <param name="propertyName">The property name that identifies the column.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid property on <typeparamref name="T"/>.
    /// </exception>
    [ExternalOnlyAttribute]
    public Column(string propertyName) : this(new PropertyName(propertyName)) { }

    /// <summary>
    /// Initializes a new <see cref="Column{T}"/> using a <see cref="PropertyName"/> wrapper.
    /// </summary>
    /// <param name="propertyName">The property name wrapper that identifies the column.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid property on <typeparamref name="T"/>.
    /// </exception>
    public Column(PropertyName propertyName)
        : base(SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyName).SingleOrDefault() ?? throw NoSuchProperty(propertyName)) => 
        PropertyName = propertyName;

    /// <summary>
    /// Leaf node: returns an empty sequence because columns do not themselves introduce parameters.
    /// </summary>
    internal override IEnumerable<Parameter> Parameters =>
        [];

    /// <summary>
    /// Leaf node: returns this column as a single-item sequence.
    /// </summary>
    internal override IEnumerable<ColumnBase> Columns =>
        [this];

    /// <summary>
    /// Produces the SQL fragment represented by this column.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated through the predicate tree to disambiguate duplicate parameter names.
    /// Not used by columns.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied parameter names that are duplicated and may require a prefix.
    /// Not used by columns.
    /// </param>
    /// <returns>
    /// The SQL-escaped column identifier (e.g., <c>[Schema].[Table].[Column]</c> or <c>[Table].[Column]</c>).
    /// </returns>
    internal override string ToSql(string prefix, IEnumerable<ParameterTag> duplicates) =>
        ColumnInfo;

    /// <summary>
    /// Leaf node: returns an empty sequence because columns contribute no parameters.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated through the predicate tree to disambiguate duplicate parameter names.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied parameter names that are duplicated and may require a prefix.
    /// </param>
    /// <returns>
    /// An empty sequence, since columns have no associated parameters.
    /// </returns>
    internal override IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates) =>
        [];
}
