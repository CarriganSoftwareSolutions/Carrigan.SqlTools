using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for SQL Parameter names,
/// providing type safety and consistent comparison semantics
/// through the <see cref="StringWrapper"/> base class.
/// </summary>
public class ParameterName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterName"/> class.
    /// </summary>
    /// <param name="name">The Parameter name string value.</param>
    public ParameterName(string? name) : base(name) { }

    /// <summary>
    /// Creates a new <see cref="ParameterName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The Parameter name to wrap.</param>
    /// <returns>
    /// A new <see cref="ParameterName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static ParameterName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new ParameterName(name);
    }
}