using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Sets;

/// <summary>
/// Represents the <see cref="ColumnCollection"/> component.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public class ColumnCollection<T> : ColumnCollectionBase<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnCollection{T}"/> class.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    public ColumnCollection(params IEnumerable<PropertyName> propertyNames) : base(propertyNames)
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnCollection{T}"/> class.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    public ColumnCollection(params IEnumerable<string> propertyNames) : base(propertyNames.Select(propertyName => new PropertyName(propertyName)))
    {
    }

    /// <summary>
    /// Gets the SupportedTypes value.
    /// </summary>
    protected override HashSet<Type> SupportedTypes =>
        DialectStatics.SupportedTypes;

    /// <summary>
    /// Executes the FromPropertyName operation.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    /// <returns>The result of the FromPropertyName operation.</returns>
    protected override ColumnCollection<T> FromPropertyName(IEnumerable<PropertyName> propertyNames) =>
        new (propertyNames);

    /// <summary>
    /// Creates a new column collection with the supplied property added.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <returns>The result of the AppendColumn operation.</returns>
    public override ColumnCollection<T> AppendColumn(PropertyName propertyName) =>
        (ColumnCollection<T>)base.AppendColumn(propertyName);

    /// <summary>
    /// Creates a new column collection with the supplied property added.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <returns>The result of the AppendColumn operation.</returns>
    public override ColumnCollection<T> AppendColumn(string propertyName) =>
        AppendColumn(new PropertyName(propertyName));

    /// <summary>
    /// Creates a new column collection with the supplied properties added.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    /// <returns>The result of the ConcatColumn operation.</returns>
    public override ColumnCollection<T> ConcatColumn(params IEnumerable<PropertyName> propertyNames) =>
        (ColumnCollection<T>)base.ConcatColumn(propertyNames);

    /// <summary>
    /// Creates a new column collection with the supplied properties added.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    /// <returns>The result of the ConcatColumn operation.</returns>
    public override ColumnCollection<T> ConcatColumn(params IEnumerable<string> propertyNames) =>
        ConcatColumn(propertyNames.Select(static propertyName => new PropertyName(propertyName)));

}
