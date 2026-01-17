using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

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
/// <see cref="ColumnValue{T}"/> validates property names and throws an exception if a property name is invalid.
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
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="propertyName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid, eligible property on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown only if a property passes validation but no matching column metadata is returned.
    /// This is not expected under normal conditions.
    /// </exception>
    public ColumnValue(PropertyName propertyName, object parameterValue) : this(CreateValue(propertyName, parameterValue))
    {
    }

    /// <summary>
    /// Private constructor used to ensure the base <see cref="Predicates"/> type is constructed
    /// with the composed predicate as a child node.
    /// </summary>
    /// <param name="equal">The composed <see cref="Equal"/> predicate.</param>
    private ColumnValue(Equal equal) : base([equal]) =>
        value = equal;

    private static Equal CreateValue(PropertyName propertyName, object parameterValue)
    {
        ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

        Column<T> left = new(propertyName);
        Parameter right = new(left.ColumnInfo.ParameterTag, parameterValue);

        return new Equal(left, right);
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
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid, eligible property on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown only if a property passes validation but no matching column metadata is returned.
    /// This is not expected under normal conditions.
    /// </exception>
    [ExternalOnly]
    public ColumnValue(string propertyName, object parameterValue) : this(new PropertyName(propertyName), parameterValue)
    {
    }

    /// <summary>
    /// Produces the SQL fragment represented by this predicate.
    /// </summary>
    /// <param name="prefix">
    /// A prefix accumulated while traversing the predicate tree; used to disambiguate
    /// duplicate parameter names when necessary.
    /// </param>
    /// <param name="branchName">
    /// The branch prefix that is prepended to the beginning of all parameter names in this predicate tree.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied parameter tags detected as duplicates. Used by leaf nodes
    /// to decide when to apply the <paramref name="prefix"/>.
    /// </param>
    /// <returns>
    /// The SQL fragment represented by this predicate, e.g., <c>[T].[Column] = @Parameter_Column</c>.
    /// </returns>
    internal override IEnumerable<SqlFragment> ToSql(string prefix, string branchName, IEnumerable<ParameterTag> duplicates)
    {
        foreach (SqlFragment fragment in value.ToSql(prefix, branchName, duplicates))
            yield return fragment;
    }
}
