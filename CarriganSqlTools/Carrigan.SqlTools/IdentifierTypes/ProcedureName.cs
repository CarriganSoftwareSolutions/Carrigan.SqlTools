using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for a SQL stored procedure name,
/// providing type safety and consistent comparison semantics through the
/// <see cref="StringWrapper"/> base class.
/// </summary>
/// <remarks>
/// This type represents the procedure identifier itself (not the schema).
/// Schema qualification, reserved word handling, invalid character rules, and length constraints
/// are validated by the SQL generator.
/// </remarks>
public class ProcedureName : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProcedureName"/> class.
    /// </summary>
    /// <param name="name">The stored procedure name string value.</param>
    public ProcedureName(string? name)
        : base(name)
    {
    }

    /// <summary>
    /// Creates a new <see cref="ProcedureName"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The stored procedure name to wrap.</param>
    /// <returns>
    /// A new <see cref="ProcedureName"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static ProcedureName? New(string? name) =>
        name.IsNullOrEmpty() ? null : new(name);
}
