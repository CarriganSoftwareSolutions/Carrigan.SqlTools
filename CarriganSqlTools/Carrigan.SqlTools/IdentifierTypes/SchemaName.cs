using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for SQL schema names,
/// providing type safety and consistent comparison semantics
/// through the <see cref="StringWrapper"/> base class.
/// </summary>
public class SchemaName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaName"/> class.
    /// </summary>
    /// <param name="name">The schema name string value.</param>
    public SchemaName(string? name) : base(name) { }

    /// <summary>
    /// Creates a new <see cref="SchemaName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The schema name to wrap.</param>
    /// <returns>
    /// A new <see cref="SchemaName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static SchemaName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new SchemaName(name);
    }
}

