using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Exceptions;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a dialect-specific SQL column expression for the specified model type.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns.</typeparam>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameter parameterName = new("Hank", "Name");
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// Equal equalName = new(columnName, parameterName);
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Where = equalName
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// WHERE ([Customer].[Name] = @Name_1)
/// ]]></code>
/// </example>
public class Column<T> : ColumnBase<T>  where T : class
{
    /// <summary>
    /// Initializes a new <see cref="ColumnBase{T}"/> using a property name.
    /// </summary>
    /// <param name="propertyName">The property name that identifies the column.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="propertyName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid, eligible property on <typeparamref name="T"/>.
    /// </exception>
    [ExternalOnly]
    public Column(string propertyName) : base(DialectStatics.SupportedTypes , new PropertyName(propertyName))
    { }

    /// <summary>
    /// Initializes a new <see cref="ColumnBase{T}"/> using a <see cref="PropertyName"/> wrapper.
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
    public Column(PropertyName propertyName) : base(DialectStatics.SupportedTypes, propertyName)
    {
    }

}
