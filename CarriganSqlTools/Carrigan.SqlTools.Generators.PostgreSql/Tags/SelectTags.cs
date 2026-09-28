using Carrigan.SqlTools.IdentifierTypes;
namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a dialect-specific collection of SQL <c>SELECT</c> tags.
/// </summary>
/// <typeparam name="T">
/// The model type whose properties resolve the SELECT projections, including any reflected projection metadata.
/// </typeparam>
public class SelectTags<T> : SelectTags where T : class
{
    /// <summary>
    /// Creates a dialect-specific select-tag collection for the requested model properties.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    public SelectTags(params IEnumerable<PropertyName> propertyNames) : base(propertyNames.Select(propertyName => SelectTagGenerator.Get<T>(propertyName)))
    {
    }
}
