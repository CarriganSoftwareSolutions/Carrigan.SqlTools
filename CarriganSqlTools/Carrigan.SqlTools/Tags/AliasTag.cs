using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents an SQL alias token used by SQL generation.
/// </summary>
/// <remarks>
/// This type wraps an <see cref="AliasName"/> value and uses <see cref="StringWrapper"/> to provide
/// consistent equality, ordering, and hashing semantics.
/// </remarks>
public class AliasTag : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AliasTag"/> class.
    /// </summary>
    /// <param name="aliasName">The alias name to associate with this tag.</param>
    public AliasTag(AliasName aliasName) : base(aliasName, StringComparison.Ordinal)
    {
    }

    /// <summary>
    /// Creates a new <see cref="AliasTag"/> instance if the specified alias name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The alias name to wrap.</param>
    /// <returns>
    /// A new <see cref="AliasTag"/> instance if <paramref name="name"/> contains a value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static AliasTag? New(AliasName? name)
    {
        if (name.IsNotNullOrEmpty())
            return new AliasTag(name);
        else
            return null;
    }
}
