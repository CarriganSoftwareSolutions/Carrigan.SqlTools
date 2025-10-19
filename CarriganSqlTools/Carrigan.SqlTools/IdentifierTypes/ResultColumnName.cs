using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for SQL result column names,
/// providing type safety and consistent comparison semantics
/// through the <see cref="StringWrapper"/> base class.
/// </summary>
public class ResultColumnName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultColumnName"/> class.
    /// </summary>
    /// <param name="name">The result column name string value.</param>
    public ResultColumnName(string? name) : base(name) { }

    /// <summary>
    /// Creates a new <see cref="ResultColumnName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The result column name to wrap.</param>
    /// <returns>
    /// A new <see cref="ResultColumnName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static ResultColumnName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new ResultColumnName(name);
    }
}