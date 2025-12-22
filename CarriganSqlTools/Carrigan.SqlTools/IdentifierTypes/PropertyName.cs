using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for a CLR property name used to reference a model property.
/// </summary>
/// <remarks>
/// In Carrigan.SqlTools, model properties represent SQL columns (table models) or parameters (procedure models).
/// This type is used by public APIs (such as predicates, ORDER BY items, and tag builders) to specify which model
/// property should be used when generating SQL.
/// <para>
/// This wrapper represents the CLR property identifier, not the SQL column identifier. If a property maps to a
/// different SQL name (for example via attributes), SQL naming rules are handled by the SQL generator.
/// </para>
/// </remarks>
public class PropertyName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyName"/> class.
    /// </summary>
    /// <param name="name">The CLR property name string value.</param>
    public PropertyName(string? name)
        : base(name)
    {
    }

    /// <summary>
    /// Creates a new <see cref="PropertyName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The CLR property name to wrap.</param>
    /// <returns>
    /// A new <see cref="PropertyName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static PropertyName? New(string? name) =>
        name.IsNullOrEmpty() ? null : new(name);
}
