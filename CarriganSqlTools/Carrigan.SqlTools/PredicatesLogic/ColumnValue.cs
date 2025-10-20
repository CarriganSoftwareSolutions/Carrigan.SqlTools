using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Data.Common;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents the predicate <c>Column = Value</c> for use in SQL <c>WHERE</c> or <c>JOIN</c> conditions.
/// This class is a convenience wrapper that reduces the boilerplate required to compare a column
/// against a constant value using the SQL equality operator (<c>=</c>).
/// </summary>
/// <remarks>
/// Property name validation is performed during construction. If the provided property name does not
/// map to a valid, eligible property on <typeparamref name="T"/>, an exception will be thrown.
/// </remarks>
/// <example>
/// <para>
/// <see cref="ColumnValue{T}"/> validates the names of the properties, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnValue<Customer> columnValue = new(nameof(Customer.Name), "Hank");
/// SqlQuery query = customerGenerator.Select(null, null, columnValue, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
public class ColumnValue<T> : Predicates
{
    /// <summary>
    /// The composed predicate (e.g., <c>[T].[Column] = @Parameter_Column</c>) that this class builds.
    /// </summary>
    protected readonly Predicates value;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnValue{T}"/> class,
    /// representing a predicate that compares a column to a constant value using
    /// the SQL equality operator (<c>=</c>).
    /// </summary>
    /// <param name="propertyName">
    /// The property name that identifies the column on the entity type <typeparamref name="T"/>.
    /// </param>
    /// <param name="parameterValue">
    /// The constant value to compare against the column in the generated SQL.
    /// </param>
    /// <remarks>
    /// This constructor validates that <paramref name="propertyName"/> maps to a valid property
    /// on <typeparamref name="T"/>. If it does not, an exception is thrown.
    /// </remarks>
    /// <exception cref="Carrigan.SqlTools.Exceptions.InvalidPropertyException{T}">
    /// Thrown if <paramref name="propertyName"/> does not exist on <typeparamref name="T"/> or is not eligible
    /// for column mapping.
    /// </exception>
    public ColumnValue(PropertyName propertyName, object parameterValue)
    {
        _ = SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyName); //called for validation.
        Column<T> left = new (propertyName);
        Parameter right = new(left.ColumnInfo.ParameterTag, parameterValue);
        value = new Equal
        (
           left,
           right
        );
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnValue{T}"/> class,
    /// representing a predicate that compares a column to a constant value using
    /// the SQL equality operator (<c>=</c>).
    /// </summary>
    /// <param name="propertyName">
    /// The property name that identifies the column on the entity type <typeparamref name="T"/>.
    /// </param>
    /// <param name="parameterValue">
    /// The constant value to compare against the column in the generated SQL.
    /// </param>
    /// <remarks>
    /// This constructor is intended for external callers and forwards to the
    /// <see cref="ColumnValue(PropertyName, object)"/> constructor after wrapping the string in a
    /// <see cref="PropertyName"/>.
    /// </remarks>
    /// <exception cref="Carrigan.SqlTools.Exceptions.InvalidPropertyException{T}">
    /// Thrown if <paramref name="propertyName"/> does not exist on <typeparamref name="T"/> or is not eligible
    /// for column mapping.
    /// </exception>
    [ExternalOnly]
    public ColumnValue(string propertyName, object parameterValue) 
        : this(new PropertyName(propertyName), parameterValue) { }

    /// <summary>
    /// Leaf node: returns all parameters used by the composed predicate.
    /// </summary>
    internal override IEnumerable<Parameter> Parameters => 
        value.Parameters;

    /// <summary>
    /// Leaf node: returns all columns used by the composed predicate.
    /// </summary>
    internal override IEnumerable<ColumnBase> Columns => 
        value.Columns;

    /// <summary>
    /// Produces the SQL fragment represented by this predicate.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated while traversing the predicate tree; used to disambiguate
    /// duplicate parameter names when necessary.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied parameter tags detected as duplicates. Used by leaf nodes
    /// to decide when to apply the <paramref name="prefix"/>.
    /// </param>
    /// <returns>
    /// The SQL fragment represented by this predicate, e.g., <c>[T].[Column] = @Parameter_Column</c>.
    /// </returns>
    internal override string ToSql(string prefix, IEnumerable<ParameterTag> duplicates) =>
        value.ToSql(prefix, duplicates);

    /// <summary>
    /// Recursively retrieves all parameters associated with this predicate as key/value pairs.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated while traversing the predicate tree; used to disambiguate
    /// duplicate parameter names when necessary.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied parameter tags detected as duplicates. Used by leaf nodes
    /// to decide when to apply the <paramref name="prefix"/>.
    /// </param>
    /// <returns>
    /// All parameters associated with this predicate, as key/value pairs of <see cref="ParameterTag"/> to value.
    /// </returns>
    internal override IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates) =>
        value.GetParameters(prefix, duplicates);
}
