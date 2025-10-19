using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for SQL table names,
/// providing type safety and consistent comparison semantics
/// through the <see cref="StringWrapper"/> base class.
/// </summary>
public class TableName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableName"/> class.
    /// </summary>
    /// <param name="name">The table name string value.</param>
    public TableName(string? name) : base(name) { }

    /// Creates a new <see cref="TableName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The table name to wrap.</param>
    /// <returns>
    /// A new <see cref="TableName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static TableName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new TableName(name);
    }
}

