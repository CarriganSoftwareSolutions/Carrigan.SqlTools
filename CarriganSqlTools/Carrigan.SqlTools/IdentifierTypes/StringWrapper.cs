using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Carrigan.SqlTools.IdentifierTypes;


//TODO: Proof read documentation and unit test
/// <summary>
/// Strongly typed string wrapper for String names
/// </summary>
public abstract class StringWrapper : IComparable<StringWrapper>, IEquatable<StringWrapper>, IEqualityComparer<StringWrapper>, IWhiteSpace
{
    protected readonly string _value;
    protected readonly StringComparison _stringComparison;

    /// <summary>
    /// Strongly typed string wrapper for String names
    /// </summary>
    /// <param name="Value">String name</param>
    internal StringWrapper(string? value, StringComparison stringComparison = StringComparison.Ordinal)
    {
        _stringComparison = stringComparison;
        _value = value ?? string.Empty;
    }

    /// <summary>
    /// Returns the SQL string representation of this <see cref="StringWrapper"/> instance.
    /// </summary>
    /// <returns>The SQL-formatted alias string.</returns>
    public override string ToString() =>
        _value;


    /// <summary>
    /// Implicitly converts a <see cref="StringWrapper"/> to its SQL string representation
    /// </summary>
    /// <param name="tag">The <see cref="StringWrapper"/> to convert.</param>
    /// <returns>The SQL-formatted as string.</returns>
    public static implicit operator string(StringWrapper tag) =>
        tag._value;

    /// <summary>
    /// Compares this instance to another <see cref="StringWrapper"/> and returns a value
    /// indicating the sort order.
    /// </summary>
    /// <param name="other">The other <see cref="StringWrapper"/> to compare.</param>
    /// <returns>
    /// A signed integer indicating relative order: 0 if equal; less than 0 if this instance
    /// precedes <paramref name="other"/>; greater than 0 if it follows.
    /// </returns>
    public int CompareTo(StringWrapper? other)
    {
        if (other is null) return 1;
        return string.Compare(_value, other._value, _stringComparison);
    }
    /// <summary>
    /// Determines whether this <see cref="StringWrapper"/> is equal to another instance.
    /// </summary>
    /// <param name="other">The other <see cref="StringWrapper"/> to compare.</param>
    /// <returns><c>true</c> if both represent the same StringWrapper; otherwise, <c>false</c>.</returns>
    public bool Equals(StringWrapper? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(_value, other._value, _stringComparison);
    }
    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="StringWrapper"/> equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        Equals(obj as StringWrapper);

    /// <summary>
    /// Returns a hash code for this <see cref="StringWrapper"/> instance.
    /// </summary>
    public override int GetHashCode() =>
        _value.ToString().GetHashCode(_stringComparison);

    /// <summary>
    /// Determines whether two <see cref="StringWrapper"/> instances are equal.
    /// </summary>
    /// <param name="x">The first <see cref="StringWrapper"/> to compare.</param>
    /// <param name="y">The second <see cref="StringWrapper"/> to compare.</param>
    /// <returns><c>true</c> if both are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(StringWrapper? x, StringWrapper? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return string.Equals(x._value, y._value, _stringComparison);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="StringWrapper"/> instance.
    /// </summary>
    /// <param name="obj">The <see cref="StringWrapper"/> for which to compute a hash code.</param>
    public int GetHashCode(StringWrapper obj) =>
        obj._value.ToString().GetHashCode(_stringComparison);

    /// <summary>
    /// Determines whether two <see cref="StringWrapper"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="StringWrapper"/> to compare.</param>
    /// <param name="right">The second <see cref="StringWrapper"/> to compare.</param>
    /// <returns><c>true</c> if both represent the same identifier; otherwise, <c>false</c>.</returns>
    /// <remarks>Equivalent to <see cref="Equals(StringWrapper?, StringWrapper?)"/>.</remarks>
    public static bool operator ==(StringWrapper? left, StringWrapper? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="StringWrapper"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="StringWrapper"/> to compare.</param>
    /// <param name="right">The second <see cref="StringWrapper"/> to compare.</param>
    /// <returns><c>true</c> if they differ; otherwise, <c>false</c>.</returns>
    public static bool operator !=(StringWrapper? left, StringWrapper? right) =>
        !(left == right);



    /// <summary>
    /// Determines if the underlying string is empty or whitespace.
    /// </summary>
    /// <returns>true is the underlying string is empty or whitespace, else false</returns>
    public bool IsWhiteSpace() =>
        _value.IsWhiteSpace();

    /// <summary>
    /// Determines if the underlying string is not empty and not whitespace.
    /// </summary>
    /// <returns>false is the underlying string is empty or whitespace, else true</returns>
    public bool IsNotWhiteSpace() =>
        IsWhiteSpace() is false;

    /// Determines if the underlying string is empty.
    /// </summary>
    /// <returns>true is the underlying string is empty, else false</returns>
    public bool IsEmpty() =>
       _value.IsEmpty();

    /// <summary>
    /// Determines if the underlying string is not empty.
    /// </summary>
    /// <returns>false is the underlying string is empty, else true</returns>
    public bool IsNotEmpty() =>
        IsEmpty() is false;
}

