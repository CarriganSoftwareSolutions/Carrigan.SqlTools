using Carrigan.SqlTools.IdentifierTypes;
namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a dialect-specific collection of SQL <c>SELECT</c> tags.
/// </summary>
/// <typeparam name="T">
/// The entity/model type that defines the table containing the columns to select.
/// </typeparam>
public class SelectTags<T> : SelectTags where T : class
{
    /// <summary>
    /// Creates a dialect-specific select-tag collection for the requested model properties.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    public SelectTags(params IEnumerable<PropertyName> propertyNames) : base (propertyNames.Select(propertyName => new SelectTag<T>(propertyName)))
    {
    }
}
