using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL Server column numeric expression for a reflected model property declared as a numeric type.
/// </summary>
/// <typeparam name="T">The entity or data model type that defines the table containing the referenced numeric column.</typeparam>
/// <remarks>
/// Note: the term numeric is being used more generally to describe types that numeric in nature, not a specific numeric type.
/// </remarks>
public class NumericColumn<T> : NumericColumnBase<T> where T : class
{
    /// <summary>
    /// Initializes a new <see cref="NumericColumn{T}"/> using a property name.
    /// </summary>
    /// <param name="propertyName">The C# property name that represents the numeric SQL column.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the referenced property is not declared as numeric type (ex short, int, long, float, double, decimal).</exception>
    [ExternalOnly]
    public NumericColumn(string propertyName) : this(new Column<T>(propertyName))
    {
    }

    /// <summary>
    /// Initializes a new <see cref="NumericColumn{T}"/> using a <see cref="PropertyName"/> wrapper.
    /// </summary>
    /// <param name="propertyName">The C# property name wrapper that represents the numeric SQL column.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the referenced property is not declared as numeric type (ex short, int, long, float, double, decimal).</exception>
    public NumericColumn(PropertyName propertyName) : this(new Column<T>(propertyName))
    {
    }

    /// <summary>
    /// Initializes a new <see cref="NumericColumn{T}"/> using a dialect-specific column expression.
    /// </summary>
    /// <param name="column">The column expression whose reflected data model property must be numeric in nature.</param>
    private NumericColumn(Column<T> column) : base(column)
    {
    }
}
