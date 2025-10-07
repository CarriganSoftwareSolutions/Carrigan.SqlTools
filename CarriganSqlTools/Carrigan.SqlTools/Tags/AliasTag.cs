using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

//TODO: Proof Read Documentation for class.
//TODO: Write unit tests
/// <summary>
/// Represents an SQL Select Alias identifier (“AS”). Implements
/// comparison and equality for use in sorting and hashed collections.
/// </summary>
/// TODO: Example Documentation
public class AliasTag : IComparable<AliasTag>, IEquatable<AliasTag>, IEqualityComparer<AliasTag>
{
    private readonly string _aliasName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AliasTag"/> class.
    /// </summary>
    /// <param name="aliasName">The alias name. If <c>null</c> or empty, only the procedure name is used.</param>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when <paramref name="aliasName"/>  fails SQL identifier validation.
    /// </exception>
    internal AliasTag(AliasName aliasName) => 
        _aliasName = aliasName;

    /// <summary>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </summary>
    /// <param name="name">Alias name</param>
    /// <returns>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </returns>
    public static AliasTag? New(AliasName? name)
    {
        if (name.IsNotNullOrEmpty())
            return new AliasTag(name);
        else
            return null;
    }

    /// <summary>
    /// Implicitly converts a <see cref="AliasTag"/> to its SQL string representation,
    /// e.g., <c>AS [Alias]</c>.
    /// </summary>
    /// <param name="tag">The <see cref="AliasTag"/> to convert.</param>
    /// <returns>The SQL-formatted as string.</returns>
    public static implicit operator string(AliasTag tag) =>
        tag._aliasName;

    /// <summary>
    /// Returns the SQL string representation of this <see cref="AliasTag"/> instance.
    /// </summary>
    /// <returns>The SQL-formatted alias string.</returns>
    public override string ToString() =>
        _aliasName;

    /// <summary>
    /// Compares this instance to another <see cref="AliasTag"/> and returns a value
    /// indicating the sort order.
    /// </summary>
    /// <param name="other">The other <see cref="AliasTag"/> to compare.</param>
    /// <returns>
    /// A signed integer indicating relative order: 0 if equal; less than 0 if this instance
    /// precedes <paramref name="other"/>; greater than 0 if it follows.
    /// </returns>
    /// <remarks>Comparison is case-sensitive and uses <see cref="StringComparison.Ordinal"/>.</remarks>
    public int CompareTo(AliasTag? other)
    {
        if (other is null) return 1; 
        return string.Compare(_aliasName, other._aliasName, StringComparison.Ordinal);
    }
    /// <summary>
    /// Determines whether this <see cref="AliasTag"/> is equal to another instance.
    /// </summary>
    /// <param name="other">The other <see cref="AliasTag"/> to compare.</param>
    /// <returns><c>true</c> if both represent the same AliasTag; otherwise, <c>false</c>.</returns>
    /// <remarks>Equality is case-sensitive and uses <see cref="StringComparison.Ordinal"/>.</remarks>
    public bool Equals(AliasTag? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(_aliasName, other._aliasName, StringComparison.Ordinal);
    }
    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="AliasTag"/> equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        Equals(obj as AliasTag);

    /// <summary>
    /// Returns a hash code for this <see cref="AliasTag"/> instance.
    /// </summary>
    /// <returns>An integer hash code computed using <see cref="StringComparison.Ordinal"/>.</returns>
    public override int GetHashCode() =>
        _aliasName.GetHashCode(StringComparison.Ordinal);

    /// <summary>
    /// Determines whether two <see cref="AliasTag"/> instances are equal.
    /// </summary>
    /// <param name="x">The first <see cref="AliasTag"/> to compare.</param>
    /// <param name="y">The second <see cref="AliasTag"/> to compare.</param>
    /// <returns><c>true</c> if both are equal; otherwise, <c>false</c>.</returns>
    /// <remarks>Equality is case-sensitive and uses <see cref="StringComparison.Ordinal"/>.</remarks>
    public bool Equals(AliasTag? x, AliasTag? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return string.Equals(x._aliasName, y._aliasName, StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="AliasTag"/> instance.
    /// </summary>
    /// <param name="obj">The <see cref="AliasTag"/> for which to compute a hash code.</param>
    /// <returns>An integer hash code computed using <see cref="StringComparison.Ordinal"/>.</returns>
    public int GetHashCode(AliasTag obj) =>
        obj._aliasName.GetHashCode(StringComparison.Ordinal);

    /// <summary>
    /// Determines whether two <see cref="AliasTag"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="AliasTag"/> to compare.</param>
    /// <param name="right">The second <see cref="AliasTag"/> to compare.</param>
    /// <returns><c>true</c> if both represent the same identifier; otherwise, <c>false</c>.</returns>
    /// <remarks>Equivalent to <see cref="Equals(AliasTag?, AliasTag?)"/>.</remarks>
    public static bool operator ==(AliasTag? left, AliasTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="AliasTag"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="AliasTag"/> to compare.</param>
    /// <param name="right">The second <see cref="AliasTag"/> to compare.</param>
    /// <returns><c>true</c> if they differ; otherwise, <c>false</c>.</returns>
    public static bool operator !=(AliasTag? left, AliasTag? right) =>
        !(left == right);
}

