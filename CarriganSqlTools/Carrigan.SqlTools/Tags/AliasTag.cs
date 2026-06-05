using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents an SQL alias token used by SQL generation.
/// </summary>
/// <remarks>
/// <para>
/// This type wraps an <see cref="AliasName"/> value and uses <see cref="StringWrapper"/> to provide
/// consistent equality, ordering, and hashing semantics.
/// </para>
/// <para>
/// Note: Inherited equality and ordering operations can throw <see cref="InvalidOperationException"/>
/// if this instance is compared against a different <see cref="StringWrapper"/> that uses a different
/// <see cref="StringComparison"/> mode.
/// </para>
/// </remarks>
public class AliasTag : StringWrapper, ISqlFragment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AliasTag"/> class.
    /// </summary>
    /// <param name="aliasName">The alias name to associate with this tag.</param>
    public AliasTag(AliasName aliasName) : base(aliasName, StringComparison.Ordinal)
    {
    }

    /// <summary>
    /// Creates a new <see cref="AliasTag"/> instance if the specified alias name is not null or empty;
    /// otherwise returns <c>null</c>.
    /// </summary>
    /// <param name="name">The alias name to wrap.</param>
    /// <returns>
    /// A new <see cref="AliasTag"/> instance if <paramref name="name"/> contains a value;
    /// otherwise, <c>null</c>.
    /// </returns>
    public static AliasTag? New(AliasName? name)
    {
        if (name.IsNotNullOrEmpty())
            return new AliasTag(name);
        else
            return null;
    }

    /// <summary>
    /// Flattens this fragment into the sequence of fragments used to render SQL text.
    /// </summary>
    /// <returns>A flattened sequence of SQL fragments that render this tag.</returns>
    public IEnumerable<ISqlFragment> Flatten()
    {
        yield return this;
    }
    /// <summary>
    /// Gets the SQL parameters contained by this fragment.
    /// </summary>
    /// <returns>An empty sequence because alias fragments do not contain SQL parameters.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters() =>
        [];

    /// <summary>
    /// Renders the alias as a dialect-quoted SQL identifier.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to quote the alias identifier.</param>
    /// <returns>The quoted alias identifier.</returns>
    public string ToSql(ISqlDialects dialect) =>
        dialect.QuoteIdentifier(this);
}
