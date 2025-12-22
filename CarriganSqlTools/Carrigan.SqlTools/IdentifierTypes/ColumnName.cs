using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for an SQL column identifier,
/// providing type safety and consistent comparison semantics through the
/// <see cref="StringWrapper"/> base class.
/// </summary>
/// <remarks>
/// In Carrigan.SqlTools, properties on a table model represent SQL columns.
/// This type is used by reflection-driven metadata and tag generation to carry
/// the column identifier as a strongly typed value.
/// <para>
/// This wrapper does not validate SQL identifier correctness (invalid characters,
/// reserved words, length constraints, etc.). Those rules are validated by the SQL generator.
/// </para>
/// </remarks>
public class ColumnName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnName"/> class.
    /// </summary>
    /// <param name="name">The column name string value.</param>
    public ColumnName(string? name) : base(name)
    {
    }

    /// <summary>
    /// Creates a new <see cref="ColumnName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The column name to wrap.</param>
    /// <returns>
    /// A new <see cref="ColumnName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static ColumnName? New(string? name) =>
        name.IsNullOrEmpty() ? null : new(name);
}
