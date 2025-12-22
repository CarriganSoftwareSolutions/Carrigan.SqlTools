using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for an SQL table identifier (the unqualified table name).
/// </summary>
/// <remarks>
/// This type represents the table name only. Schema qualification is represented separately
/// (for example, via <see cref="SchemaName"/>), and SQL generation is responsible for rendering
/// qualified identifiers.
/// <para>
/// This wrapper does not validate SQL identifier correctness (invalid characters, reserved words,
/// length constraints, etc.). SQL identifier rules are validated by the SQL generator.
/// </para>
/// </remarks>
public class TableName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableName"/> class.
    /// </summary>
    /// <param name="name">The table name string value.</param>
    public TableName(string? name)
        : base(name)
    {
    }

    /// <summary>
    /// Creates a new <see cref="TableName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The table name to wrap.</param>
    /// <returns>
    /// A new <see cref="TableName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static TableName? New(string? name) =>
        name.IsNullOrEmpty() ? null : new(name);
}
