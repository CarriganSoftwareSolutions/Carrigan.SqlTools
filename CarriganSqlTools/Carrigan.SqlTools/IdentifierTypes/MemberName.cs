using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Initializes a new instance of the <see cref="ColumnName"/> class.
/// </summary>
/// <param name="name">The column name string value.</param>
internal class MemberName : StringWrapper
{    
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnName"/> class.
    /// </summary>
    /// <param name="name">The column name string value.</param>
    internal MemberName(string? name) : base(name) { }

    /// <summary>
    /// Creates a new <see cref="ColumnName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The column name to wrap.</param>
    /// <returns>
    /// A new <see cref="ColumnName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    internal static MemberName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new MemberName(name);
    }
}