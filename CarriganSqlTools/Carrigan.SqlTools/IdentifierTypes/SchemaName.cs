using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for a SQL schema identifier (for example, <c>dbo</c>).
/// </summary>
/// <remarks>
/// This type is used when qualifying table or stored procedure identifiers (for example,
/// <c>[dbo].[MyTable]</c> or <c>[dbo].[MyProcedure]</c>).
/// <para>
/// This wrapper does not validate SQL identifier correctness (invalid characters, reserved words,
/// length constraints, etc.). SQL identifier rules are validated by the SQL generator.
/// </para>
/// </remarks>
public class SchemaName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaName"/> class.
    /// </summary>
    /// <param name="name">The schema name string value.</param>
    public SchemaName(string? name)
        : base(name)
    {
    }

    /// <summary>
    /// Creates a new <see cref="SchemaName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The schema name to wrap.</param>
    /// <returns>
    /// A new <see cref="SchemaName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static SchemaName? New(string? name) =>
        name.IsNullOrEmpty() ? null : new(name);
}
