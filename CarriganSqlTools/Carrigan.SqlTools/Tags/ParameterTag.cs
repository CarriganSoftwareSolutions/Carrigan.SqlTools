using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.RegularExpressions;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a SQL parameter identifier, or “tag,” used in query generation.
/// A parameter tag can optionally include a prefix and/or an index,
/// and is rendered as a single string with parts joined by underscores (e.g., <c>Prefix_Column_Index</c>).
/// </summary>
/// <remarks>
/// This class implements <see cref="IComparable{ParameterTag}"/>, <see cref="IEquatable{ParameterTag}"/>,
/// and <see cref="IEqualityComparer{ParameterTag}"/> to support sorting, equality checks,
/// and use in collections that rely on hashing.
/// </remarks>
/// <example>
/// <para>
///  Using Parameter Example
/// </para>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.SqlGenerators;
/// 
/// [Identifier("UpdateThing", "schema")]
/// public class ProcedureExec
/// {
///     [Parameter("SomeValue")]
///     public string? ValueColumn { get; set; }
/// }
/// 
/// SqlGenerator<ProcedureExec> procedureExecGenerator = new();
/// 
/// ProcedureExec procedureExec = new()
/// {
///     ValueColumn = "DangItBobby"
/// };
/// SqlQuery query = procedureExecGenerator.Procedure(procedureExec);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// [schema].[UpdateThing]
/// ]]></code>
/// </example>
public class ParameterTag : IComparable<ParameterTag>, IEquatable<ParameterTag>, IEqualityComparer<ParameterTag>
{
    /// <summary>
    /// The base name of the parameter. Must not be <c>null</c>, empty, or white space.
    /// </summary>
    private readonly string _parameterBaseName;
    /// <summary>
    /// An optional prefix to prepend to the parameter name.
    /// </summary>
    private readonly string? _prefix;
    /// <summary>
    /// An optional index to append to the parameter name.
    /// </summary>
    private readonly string? _index;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterTag"/> class.
    /// </summary>
    /// <param name="prefix">
    /// An optional prefix to prepend to the parameter name.
    /// </param>
    /// <param name="parameterName">
    /// The base name of the parameter. Must not be <c>null</c>, empty, or white space.
    /// </param>
    /// <param name="index">
    /// An optional index to append to the parameter name.
    /// </param>
    /// <exception cref="InvalidParameterIdentifierException">
    /// Thrown when <paramref name="parameterName"/> is <c>null</c>, empty, or white space.
    /// </exception>
    internal ParameterTag(string? prefix, string parameterName, string? index)
    {
        if (SqlParameterPattern.Fails(parameterName))
            throw new InvalidParameterIdentifierException(ToString()); 

        _parameterBaseName = parameterName;
        _prefix = prefix;
        _index = index;

        if (SqlParameterPattern.Fails(ToString()))
            throw new InvalidParameterIdentifierException(ToString()); 
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterTag"/> class.
    /// </summary>
    /// <param name="parameterName">
    /// The base name of the parameter. Must not be <c>null</c>, empty, or white space.
    /// </param>
    /// <exception cref="InvalidParameterIdentifierException">
    /// Thrown when <paramref name="parameterName"/> is <c>null</c>, empty, or white space.
    /// </exception>
    public ParameterTag(string parameterName)
    {
        if (SqlParameterPattern.Fails(parameterName))
            throw new InvalidParameterIdentifierException(ToString()); 

        _parameterBaseName = parameterName;
        _prefix = null;
        _index = null;

        if (SqlParameterPattern.Fails(ToString()))
            throw new InvalidParameterIdentifierException(ToString()); 
    }

    /// <summary>
    /// Implicitly converts a <see cref="ParameterTag"/> to its SQL string representation,
    /// combining the prefix, base name, and index (if present) with underscores.
    /// </summary>
    /// <param name="value">The <see cref="ParameterTag"/> to convert.</param>
    /// <returns>
    /// A SQL parameter name as a string, for example <c>Prefix_Column_Index</c>.
    /// </returns>
    public static implicit operator string(ParameterTag value)
    {
        IEnumerable<string?> stringParts = new List<string?>([value._prefix, value._parameterBaseName, value._index])
            .Where(part => part.IsNotNullOrWhiteSpace());
        return string.Join('_',  stringParts);
    }

    /// <summary>
    /// Returns the SQL string representation of this <see cref="ParameterTag"/> instance,
    /// which is equivalent to the result of the implicit conversion to <see cref="string"/>.
    /// </summary>
    /// <returns>
    /// A SQL parameter name as a string, for example <c>Prefix_Column_Index</c>.
    /// </returns>
    public override string ToString() =>
        (string)this;

    /// <summary>
    /// Compares this <see cref="ParameterTag"/> instance with another
    /// to determine their relative sort order.
    /// </summary>
    /// <param name="other">
    /// The <see cref="ParameterTag"/> to compare with the current instance.
    /// </param>
    /// <returns>
    /// A signed integer that indicates the relative order of the objects:
    /// <c>0</c> if they are equal, a negative value if this instance precedes
    /// <paramref name="other"/>, and a positive value if this instance follows
    /// <paramref name="other"/>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and uses
    /// <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// If <paramref name="other"/> is <c>null</c>, this instance is considered greater.
    /// </remarks>
    public int CompareTo(ParameterTag? other)
    {
        if (other is null) return 1;
        return string.Compare(this, (string)other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the current <see cref="ParameterTag"/> is equal to another
    /// <see cref="ParameterTag"/> instance.
    /// </summary>
    /// <param name="other">
    /// The <see cref="ParameterTag"/> to compare with this instance.
    /// </param>
    /// <returns>
    /// <c>true</c> if both instances represent the same parameter (case-insensitive);
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and uses
    /// <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </remarks>
    public bool Equals(ParameterTag? other)
    {
        if (other is null) return false;
        return string.Equals((string)this, (string)other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current
    /// <see cref="ParameterTag"/> instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="obj"/> is a <see cref="ParameterTag"/> and
    /// represents the same parameter (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and delegates to
    /// <see cref="Equals(ParameterTag?)"/>.
    /// </remarks>
    public override bool Equals(object? obj) =>
        obj is ParameterTag ct && Equals(ct);

    /// <summary>
    /// Returns a hash code for this <see cref="ParameterTag"/> instance,
    /// consistent with the case-insensitive comparison used in <see cref="Equals(ParameterTag?)"/>.
    /// </summary>
    /// <returns>
    /// An integer hash code based on the string representation of this parameter,
    /// computed using <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </returns>
    public override int GetHashCode() =>
        ((string) this).GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Determines whether two <see cref="ParameterTag"/> instances are equal.
    /// </summary>
    /// <param name="x">The first <see cref="ParameterTag"/> to compare.</param>
    /// <param name="y">The second <see cref="ParameterTag"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if both <paramref name="x"/> and <paramref name="y"/> represent the same
    /// parameter (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and delegates to
    /// <see cref="Equals(ParameterTag?)"/>.
    /// </remarks>
    public bool Equals(ParameterTag? x, ParameterTag? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="ParameterTag"/> instance,
    /// consistent with the case-insensitive comparison used in <see cref="Equals(ParameterTag?, ParameterTag?)"/>.
    /// </summary>
    /// <param name="obj">The <see cref="ParameterTag"/> for which to compute a hash code.</param>
    /// <returns>
    /// An integer hash code for <paramref name="obj"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="obj"/> is <c>null</c>.
    /// </exception>
    public int GetHashCode(ParameterTag obj) =>
        obj is null ? throw new ArgumentNullException(nameof(obj)) : obj.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="ParameterTag"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="ParameterTag"/> to compare.</param>
    /// <param name="right">The second <see cref="ParameterTag"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if both <paramref name="left"/> and <paramref name="right"/> represent
    /// the same parameter (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and equivalent to calling
    /// <see cref="Equals(ParameterTag?, ParameterTag?)"/>.
    /// </remarks>
    public static bool operator ==(ParameterTag? left, ParameterTag? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="ParameterTag"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="ParameterTag"/> to compare.</param>
    /// <param name="right">The second <see cref="ParameterTag"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> do not represent
    /// the same parameter (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and is the logical negation of
    /// <see cref="operator ==(ParameterTag?, ParameterTag?)"/>.
    /// </remarks>
    public static bool operator !=(ParameterTag? left, ParameterTag? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether this <see cref="ParameterTag"/> is empty.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the parameter name is <c>null</c>, empty, or consists only of white space;
    /// otherwise, <c>false</c>.
    /// </returns>
    internal bool IsEmpty() =>
        ((string)this).IsNullOrWhiteSpace();

    /// <summary>
    /// Creates a new <see cref="ParameterTag"/> with the specified text prepended
    /// to the existing prefix (if any).
    /// </summary>
    /// <param name="textToAppend">
    /// The text to prepend to the prefix. If the current prefix is empty,
    /// this value becomes the new prefix.
    /// </param>
    /// <returns>
    /// A new <see cref="ParameterTag"/> with the updated prefix.
    /// </returns>
    internal ParameterTag PrefixPrepend(string? textToAppend)
    {
        if (_prefix.IsNullOrWhiteSpace())
            return new(textToAppend, _parameterBaseName, _index);
        else
            return new($"{textToAppend}_{_prefix}", _parameterBaseName, _index);
    }

    /// <summary>
    /// Creates a new <see cref="ParameterTag"/> with the specified text appended
    /// to the existing prefix (if any).
    /// </summary>
    /// <param name="textToAppend">
    /// The text to append to the prefix. If the current prefix is empty,
    /// this value becomes the new prefix.
    /// </param>
    /// <returns>
    /// A new <see cref="ParameterTag"/> with the updated prefix.
    /// </returns>
    internal ParameterTag PrefixAppend(string? textToAppend)
    {
        if (_prefix.IsNullOrWhiteSpace())
            return new(textToAppend, _parameterBaseName, _index);
        else
            return new($"{_prefix}_{textToAppend}", _parameterBaseName, _index);
    }

    /// <summary>
    /// Creates a new <see cref="ParameterTag"/> with the specified index appended
    /// to the current parameter identifier.
    /// </summary>
    /// <param name="newIndex">
    /// The index value to append. Must not be <c>null</c>, empty, or white space.
    /// </param>
    /// <returns>
    /// A new <see cref="ParameterTag"/> that includes the specified index.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if an index has already been defined on this <see cref="ParameterTag"/>.
    /// </exception>
    internal ParameterTag AddIndex(string? newIndex)
    {
        if (_index.IsNullOrWhiteSpace())
            return new(_prefix, _parameterBaseName, newIndex);
        else
            throw new ArgumentException("Index was already defined on the Parameter");
    }
}