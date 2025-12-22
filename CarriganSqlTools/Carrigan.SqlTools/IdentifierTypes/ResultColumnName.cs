using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for a result set column name.
/// </summary>
/// <remarks>
/// A result set column name is the name exposed by a query result (for example, a projected column name
/// or an alias used in an <c>AS</c> clause). This may differ from the underlying table column identifier.
/// <para>
/// This wrapper does not validate SQL identifier correctness (invalid characters, reserved words,
/// length constraints, etc.). Result column naming rules and ambiguity checks are handled by the SQL generator
/// and result materialization components.
/// </para>
/// </remarks>
public class ResultColumnName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultColumnName"/> class.
    /// </summary>
    /// <param name="name">The result column name string value.</param>
    public ResultColumnName(string? name)
        : base(name)
    {
    }

    /// <summary>
    /// Creates a new <see cref="ResultColumnName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The result column name to wrap.</param>
    /// <returns>
    /// A new <see cref="ResultColumnName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static ResultColumnName? New(string? name) =>
        name.IsNullOrEmpty() ? null : new(name);
}
