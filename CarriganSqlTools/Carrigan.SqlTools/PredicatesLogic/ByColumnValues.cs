using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a simple predicate in which a column is compared to a specific value (i.e., <c>Column = Value</c>).
/// </summary>
/// <remarks>
/// This class functions as a shorthand or alias for <see cref="ColumnValue{T}"/>, designed to reduce the amount of code
/// needed for common equality comparisons in SQL <c>WHERE</c> or <c>JOIN</c> clauses.
/// </remarks>
/// <typeparam name="T">
/// The entity or data model type that defines the table containing the column to compare.
/// </typeparam>
[Obsolete("Use ColumnValues<T> instead.")]
public class ByColumnValues<T> : ColumnValue<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ByColumnValues{T}"/> class,
    /// representing a predicate that equates a property to a specific value.
    /// </summary>
    /// <param name="propertyName">The property name representing the column.</param>
    /// <param name="value">The value to compare against.</param>
    [Obsolete("Use ColumnValues<T> instead.")]
    public ByColumnValues(PropertyName propertyName, object value) : base(propertyName, value)
    {
    }
}
