using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for SQL alias names (the identifier used in an <c>AS</c> clause),
/// providing type safety and consistent comparison semantics through the <see cref="StringWrapper"/> base class.
/// </summary>
/// <remarks>
/// This type is used to represent the projected result column name when a SELECT list item is aliased
/// (for example: <c>[Table].[Column] AS MyAlias</c>).
/// <para>
/// This wrapper performs no SQL identifier validation beyond the behavior of <see cref="StringWrapper"/>.
/// SQL identifier rules (such as invalid characters, reserved words, and length constraints) are validated
/// by the SQL generator.
/// </para>
/// </remarks>
public class AliasName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AliasName"/> class.
    /// </summary>
    /// <param name="name">The alias name string value.</param>
    public AliasName(string? name) : base(name)
    {
    }

    /// <summary>
    /// Creates a new <see cref="AliasName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The alias name to wrap.</param>
    /// <returns>
    /// A new <see cref="AliasName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static AliasName? New(string? name) =>
        name.IsNullOrEmpty() ? null : new(name);
}
