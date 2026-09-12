using System.Numerics;
using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents an SQL alias token used by SQL generation.
/// </summary>
/// <remarks>
/// Equality and hashing are based on the underlying <see cref="AliasName"/> value.
/// </remarks>
public class AliasTag : IEquatable<AliasTag>, IEqualityOperators<AliasTag, AliasTag, bool>, ISqlFragment, IWhiteSpace, IEmpty
{
    private readonly AliasName _aliasName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AliasTag"/> class.
    /// </summary>
    /// <param name="aliasName">The alias name to associate with this tag.</param>
    public AliasTag(AliasName aliasName)
    {
        ArgumentNullException.ThrowIfNull(aliasName);
        _aliasName = aliasName;
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
    /// Returns the alias name represented by this tag.
    /// </summary>
    /// <returns>The underlying alias name.</returns>
    public override string ToString() =>
        _aliasName.ToString();

    /// <summary>
    /// Determines whether this instance represents the same alias as another <see cref="AliasTag"/>.
    /// </summary>
    /// <param name="other">The other alias tag to compare.</param>
    /// <returns><c>true</c> when both tags represent equal alias names; otherwise, <c>false</c>.</returns>
    public bool Equals(AliasTag? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return _aliasName.Equals(other._aliasName);
    }

    /// <summary>
    /// Determines whether the specified object represents the same alias as this instance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> when <paramref name="obj"/> is an equivalent <see cref="AliasTag"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        Equals(obj as AliasTag);

    /// <summary>
    /// Returns a hash code for this alias tag.
    /// </summary>
    /// <returns>A hash code consistent with <see cref="Equals(AliasTag?)"/>.</returns>
    public override int GetHashCode() =>
        _aliasName.GetHashCode();

    /// <summary>
    /// Determines whether two alias tags represent the same alias name.
    /// </summary>
    public static bool operator ==(AliasTag? left, AliasTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two alias tags represent different alias names.
    /// </summary>
    public static bool operator !=(AliasTag? left, AliasTag? right) =>
        !(left == right);

    /// <summary>
    /// Flattens this fragment into the sequence of fragments used to render SQL text.
    /// </summary>
    /// <param name="dialect">The SQL dialect used when flattening this fragment.</param>
    /// <returns>A flattened sequence of SQL fragments that render this tag.</returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect)
    {
        yield return this;
    }

    /// <summary>
    /// Gets the SQL parameters contained by this fragment.
    /// </summary>
    /// <param name="dialect">The SQL dialect used when inspecting this fragment.</param>
    /// <returns>An empty sequence because alias fragments do not contain SQL parameters.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        [];

    /// <summary>
    /// Renders the alias as a dialect-quoted SQL identifier.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to quote the alias identifier.</param>
    /// <returns>The quoted alias identifier.</returns>
    public string ToSql(ISqlDialects dialect) =>
        dialect.QuoteIdentifier(_aliasName.ToString());

    /// <summary>
    /// Indicates whether the alias name is null, empty, or consists only of whitespace characters.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the alias name is null, empty, or whitespace; otherwise, <c>false</c>.
    /// </returns>
    public bool IsWhiteSpace() =>
        _aliasName.IsWhiteSpace();

    /// <summary>
    /// Indicates whether the alias name is not null, not empty, and contains at least one non-whitespace character.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the alias name is not null, not empty, and contains non-whitespace characters; otherwise, <c>false</c>.
    /// </returns>
    public bool IsNotWhiteSpace() =>
        _aliasName.IsNotWhiteSpace();

    /// <summary>
    /// Indicates whether the alias name is null or empty.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the alias name is null or empty; otherwise, <c>false</c>.
    /// </returns>
    public bool IsEmpty() =>
        _aliasName.IsEmpty();

    /// <summary>
    /// Indicates whether the alias name is not null and not empty.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the alias name is not null and not empty; otherwise, <c>false</c>.
    /// </returns>
    public bool IsNotEmpty() =>
        _aliasName.IsNotEmpty();
}
