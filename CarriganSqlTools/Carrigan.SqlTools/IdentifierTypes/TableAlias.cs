using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for SQL alias names (the identifier used in an <c>AS</c> clause),
/// providing type safety and consistent comparison semantics through the <see cref="TextWrapper"/> base class.
/// </summary>
/// <remarks>
/// This type is used to represent the projected result column name when a SELECT list item is aliased
/// (for example: <c>[Table].[Column] AS MyAlias</c>).
/// <para>
/// This wrapper performs no SQL identifier validation beyond the behavior of <see cref="TextWrapper"/>.
/// SQL identifier rules (such as invalid characters, reserved words, and length constraints) are validated
/// by the SQL generator.
/// </para>
/// </remarks>
public class TableAlias : TextWrapper, ISqlFragment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableAlias"/> class.
    /// </summary>
    /// <param name="name">The alias name string value.</param>
    public TableAlias(string? name) : base(name, StringComparison.OrdinalIgnoreCase)
    {
    }

    /// <summary>
    /// Creates a new <see cref="TableAlias"/> instance if the specified name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The alias name to wrap.</param>
    /// <returns>
    /// A new <see cref="TableAlias"/> instance if <paramref name="name"/> contains a valid value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static TableAlias? New(string? name) =>
        name.IsNullOrEmpty() ? null : new(name);

    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect) => throw new NotImplementedException();
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) => throw new NotImplementedException();
    public string ToSql(ISqlDialects dialect) =>
        dialect.RenderTableAlias(this);
}
