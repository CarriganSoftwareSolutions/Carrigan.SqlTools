using Carrigan.Core.Attributes;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;

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
/// SELECT "Customer".*
/// FROM "Customer"
/// WHERE ("Customer"."Name" = $1)
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

    #region Implicitly Convert all Column<T>s into the numeric and bool Inheritance Chain
    /// <summary>
    /// Implicitly converts a <see cref="Column{T}"/> to a <see cref="NumericColumn{T}"/>.
    /// </summary>
    /// <param name="column">
    /// The <see cref="Column{T}"/> instance to convert.
    /// </param>
    [TypeSafetyLoss]
    public static implicit operator NumericColumn<T>(Column<T> column) =>
        new(column);

    /// <summary>
    /// Implicitly converts a <see cref="Column{T}"/> to a <see cref="NumericColumn{T}"/>.
    /// </summary>
    /// <param name="column">
    /// The <see cref="Column{T}"/> instance to convert.
    /// </param>
    [TypeSafetyLoss]
    public static implicit operator NumericColumnBase<T>(Column<T> column) =>
        new NumericColumn<T>(column);

    /// <summary>
    /// Implicitly converts a <see cref="Column{T}"/> to a <see cref="NumericExpression"/>.
    /// </summary>
    /// <param name="column">
    /// The <see cref="Column{T}"/> instance to convert.
    /// </param>
    [TypeSafetyLoss]
    public static implicit operator NumericExpression(Column<T> column) =>
        new NumericColumn<T>(column);

    /// <summary>
    /// Implicitly converts a <see cref="Column{T}"/> to a <see cref="BooleanColumn{T}"/>.
    /// </summary>
    /// <param name="column">
    /// The <see cref="Column{T}"/> instance to convert.
    /// </param>
    [TypeSafetyLoss]
    public static implicit operator BooleanColumn<T>(Column<T> column) =>
        new(column);

    /// <summary>
    /// Implicitly converts a <see cref="Column{T}"/> to a <see cref="BooleanColumnBase{T}"/>.
    /// </summary>
    /// <param name="column">
    /// The <see cref="Column{T}"/> instance to convert.
    /// </param>
    [TypeSafetyLoss]
    public static implicit operator BooleanColumnBase<T>(Column<T> column) =>
        new BooleanColumn<T>(column);

    /// <summary>
    /// Implicitly converts a <see cref="Column{T}"/> to a <see cref="Predicates"/>.
    /// </summary>
    /// <param name="column">
    /// The <see cref="Column{T}"/> instance to convert.
    /// </param>
    [TypeSafetyLoss]
    public static implicit operator Predicates(Column<T> column) =>
        new BooleanColumn<T>(column);
    #endregion

    #region Implicitly Convert numeric and bool Inheritance Chain to Column<T>, but not the final concrete numeric bool class.
    /// <summary>
    /// Implicitly converts a <see cref="NumericColumn{T}"/> to a <see cref="Column{T}"/>.
    /// </summary>
    /// <param name="numericColumnBase">
    /// The <see cref="NumericColumn{T}"/> instance to convert.
    /// </param>
    public static implicit operator Column<T>(NumericColumnBase<T> numericColumnBase) =>
        new(numericColumnBase.PropertyName);

    /// <summary>
    /// Implicitly converts a <see cref="BooleanColumn{T}"/> to a <see cref="Column{T}"/>.
    /// </summary>
    /// <param name="booleanColumn">
    /// The <see cref="BooleanColumn{T}"/> instance to convert.
    /// </param>
    public static implicit operator Column<T>(BooleanColumn<T> booleanColumn) =>
        new(booleanColumn.PropertyName);

    /// <summary>
    /// Implicitly converts a <see cref="BooleanColumnBase{T}"/> to a <see cref="Column{T}"/>.
    /// </summary>
    /// <param name="booleanColumn">
    /// The <see cref="BooleanColumnBase{T}"/> instance to convert.
    /// </param>
    public static implicit operator Column<T>(BooleanColumnBase<T> booleanColumn) =>
        new(booleanColumn.PropertyName);
    #endregion
}
