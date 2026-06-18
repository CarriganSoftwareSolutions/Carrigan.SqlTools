using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a PostgreSQL boolean column predicate for a reflected model property declared as <see cref="bool"/> or nullable <see cref="bool"/>.
/// </summary>
/// <typeparam name="T">The entity or data model type that defines the table containing the referenced boolean column.</typeparam>
public class BooleanColumn<T> : BooleanColumnBase<T> where T : class
{
    /// <summary>
    /// Initializes a new <see cref="BooleanColumn{T}"/> using a property name.
    /// </summary>
    /// <param name="propertyName">The C# property name that represents the boolean SQL column.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the referenced property is not declared as <see cref="bool"/> or nullable <see cref="bool"/>.</exception>
    [ExternalOnly]
    public BooleanColumn(string propertyName) : this(new Column<T>(propertyName))
    {
    }

    /// <summary>
    /// Initializes a new <see cref="BooleanColumn{T}"/> using a <see cref="PropertyName"/> wrapper.
    /// </summary>
    /// <param name="propertyName">The C# property name wrapper that represents the boolean SQL column.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the referenced property is not declared as <see cref="bool"/> or nullable <see cref="bool"/>.</exception>
    public BooleanColumn(PropertyName propertyName) : this(new Column<T>(propertyName))
    {
    }

    /// <summary>
    /// Initializes a new <see cref="BooleanColumn{T}"/> using a dialect-specific column expression.
    /// </summary>
    /// <param name="column">The column expression whose reflected data model property must be boolean.</param>
    private BooleanColumn(Column<T> column) : base(column)
    {
    }
}
