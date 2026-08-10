using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using System.Numerics;

//TODO: unit tests.

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL Server column numeric expression for a reflected model property declared as a numeric type.
/// </summary>
/// <typeparam name="modelT">The entity or data model type that defines the table containing the referenced numeric column.</typeparam>
/// <typeparam name="propertyT">
/// The numeric type of the reflected model property that maps to the SQL column.
/// </typeparam>
/// <remarks>
/// Note: the term numeric is being used more generally to describe types that numeric in nature, not a specific numeric type.
/// </remarks>
public sealed class NumericColumn<modelT, propertyT> : NumericColumn<modelT> 
    where modelT : class
    where propertyT : INumber<propertyT>
{
    /// <summary>
    /// Initializes a new <see cref="NumericColumn{modelT, propertyT}"/> using a property name.
    /// </summary>
    /// <param name="propertyName">The C# property name that represents the numeric SQL column.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the referenced property is not declared as numeric type (ex short, int, long, float, double, decimal).</exception>
    [ExternalOnly]
    public NumericColumn(string propertyName) : this(new Column<modelT>(propertyName))
    {
    }

    /// <summary>
    /// Initializes a new <see cref="NumericColumn{modelT, propertyT}"/> using a <see cref="PropertyName"/> wrapper.
    /// </summary>
    /// <param name="propertyName">The C# property name wrapper that represents the numeric SQL column.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the referenced property is not declared as numeric type (ex short, int, long, float, double, decimal).</exception>
    public NumericColumn(PropertyName propertyName) : this(new Column<modelT>(propertyName))
    {
    }

    /// <summary>
    /// Initializes a new <see cref="NumericColumn{modelT, propertyT}"/> using a dialect-specific column expression.
    /// </summary>
    /// <param name="column">The column expression whose reflected data model property must be numeric in nature.</param>
    private NumericColumn(Column<modelT> column) : base(column)
    {
    }
}

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
    internal NumericColumn(PropertyName propertyName) : base(new Column<modelT>(propertyName))
    {
    }

    /// <summary>
    /// Creates a new <see cref="NumericColumn{modelT, propertyT}"/> instance for the specified property name.
    /// </summary>
    /// <typeparam name="propertyT">
    /// The numeric type of the reflected model property that maps to the SQL column.
    /// </typeparam>
    /// <param name="propertyName">
    /// The C# property name wrapper that represents the numeric SQL column.
    /// </param>
    /// <returns>
    /// A new <see cref="NumericColumn{modelT, propertyT}"/> instance for the specified property name.
    /// </returns>
    public static NumericColumn<modelT, propertyT> New<propertyT>(PropertyName propertyName)
        where propertyT : INumber<propertyT> =>
        new(propertyName);

    /// <summary>
    /// Initializes a new <see cref="NumericColumn{modelT}"/> instance using a dialect-specific column expression.
    /// </summary>
    /// <param name="column">
    /// The column expression whose reflected data model property must be numeric in nature.
    /// </param>
    protected NumericColumn(Column<modelT> column) : base(column)
    {
    }
}
