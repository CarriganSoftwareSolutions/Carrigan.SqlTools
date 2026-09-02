using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;

//TODO: unit tests.

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL Server column numeric expression for a reflected model property declared as a numeric type.
/// </summary>
/// <typeparam name="modelT">The entity or data model type that defines the table containing the referenced numeric column.</typeparam>
/// <remarks>
/// Note: the term numeric is being used more generally to describe types that numeric in nature, not a specific numeric type.
/// </remarks>
public class NumericColumn<modelT> : NumericColumnBase<modelT>
    where modelT : class
{
    /// <summary>
    /// Initializes a new <see cref="NumericColumn{modelT}"/> using a <see cref="PropertyName"/> wrapper.
    /// </summary>
    /// <param name="propertyName">The C# property name wrapper that represents the numeric SQL column.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the referenced property is not declared as numeric type (ex short, int, long, float, double, decimal).</exception>
    public NumericColumn(PropertyName propertyName) : base(new Column<modelT>(propertyName))
    {
    }

    /// <summary>
    /// Initializes a new <see cref="NumericColumn{modelT}"/> using a property name.
    /// </summary>
    /// <param name="propertyName">
    /// The C# property name that represents the numeric SQL column.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the referenced property is not declared as numeric type (ex short, int, long, float, double, decimal).</exception>
    [ExternalOnly]
    public NumericColumn(string propertyName) : this(new PropertyName(propertyName))
    {
    }

    /// <summary>
    /// Initializes a new <see cref="NumericColumn{modelT}"/> instance using a dialect-specific column expression.
    /// </summary>
    /// <param name="column">
    /// The column expression whose reflected data model property must be numeric in nature.
    /// </param>
    internal NumericColumn(Column<modelT> column) : base(column)
    {
        if (column.ColumnInfo.Type.IsNumericType() is false)
            throw new NonNumericValueException(column.ColumnInfo.Type);
    }

    #region implicit operators to allow the concrete Numeric Column class to act as a regular column.
    /// <summary>
    /// Implicitly converts a <see cref="NumericColumn{modelT}"/> to a <see cref="Column{T}"/>.
    /// </summary>
    /// <param name="numericColumn">
    /// The <see cref="NumericColumn{modelT}"/> instance to convert.
    /// </param>
    public static implicit operator Column<modelT>(NumericColumn<modelT> numericColumn) =>
        new(numericColumn.PropertyName);

    #endregion
}
