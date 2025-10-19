using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for property names,
/// providing type safety and consistent comparison semantics
/// through the <see cref="StringWrapper"/> base class.
/// </summary>
public class PropertyName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyName"/> class.
    /// </summary>
    /// <param name="name">The property name string value.</param>

    public PropertyName(string? name) : base(name) { }

    /// <summary>
    /// Creates a new <see cref="PropertyName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The property name to wrap.</param>
    /// <returns>
    /// A new <see cref="PropertyName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static PropertyName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new PropertyName(name);
    }
}
