using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for a CLR member name (for example, a property name),
/// providing type safety and consistent comparison semantics through the <see cref="StringWrapper"/> base class.
/// </summary>
/// <remarks>
/// This type is used internally to retain the originating CLR member name when generating SQL,
/// primarily for error reporting and validation messages.
/// <para>
/// This wrapper does not validate SQL identifier rules. SQL identifier validation is performed by the SQL generator.
/// </para>
/// </remarks>
internal sealed class MemberName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemberName"/> class.
    /// </summary>
    /// <param name="name">The CLR member name string value.</param>
    internal MemberName(string? name)
        : base(name)
    {
    }

    /// <summary>
    /// Creates a new <see cref="MemberName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The member name to wrap.</param>
    /// <returns>
    /// A new <see cref="MemberName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    internal static MemberName? New(string? name) =>
        name.IsNullOrEmpty() ? null : new(name);
}
