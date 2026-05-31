using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;

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
public class ColumnValue<T> : ColumnValueBase<T> where T : class
{
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
    public ColumnValue(PropertyName propertyName, object? parameterValue) : base(CreateValue(propertyName), parameterValue)
    {
    }

    /// <summary>
    /// Creates a <see cref="ColumnBase{T}"/> instance for the given <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">
    /// The property name that identifies the column on the entity type <typeparamref name="T"/>.
    /// </param>
    /// <returns></returns>
    private static ColumnBase<T> CreateValue(PropertyName propertyName)
    {
        ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

        return new Column<T>(propertyName);
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
}
