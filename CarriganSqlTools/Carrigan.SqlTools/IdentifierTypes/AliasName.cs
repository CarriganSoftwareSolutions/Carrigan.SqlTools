using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for SQL alias names,
/// providing type safety and consistent comparison semantics
/// through the <see cref="StringWrapper"/> base class.
/// </summary>
public class AliasName : StringWrapper
{    
    /// <summary>
    /// Initializes a new instance of the <see cref="AliasName"/> class.
    /// </summary>
    /// <param name="name">The alias name string value.</param>
    public AliasName(string? name) : base(name) { }

    /// <summary>
    /// Creates a new <see cref="AliasName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The alias name to wrap.</param>
    /// <returns>
    /// A new <see cref="AliasName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static AliasName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new AliasName(name);
    }
}