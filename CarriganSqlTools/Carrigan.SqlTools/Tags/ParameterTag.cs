using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.RegularExpressions;
using Carrigan.SqlTools.Types;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a SQL parameter identifier, or “tag,” used in query generation.
/// A parameter tag can optionally include a prefix and/or an index, and is rendered
/// as a single string with parts joined by underscores (for example, <c>Prefix_Column_Index</c>).
/// </summary>
/// <remarks>
/// This class implements <see cref="IComparable{T}"/>, <see cref="IEquatable{T}"/>,
/// and <see cref="IEqualityComparer{T}"/> to support sorting, equality checks,
/// and use in collections that rely on hashing. Parameter names may be provided
/// explicitly (for example, via <see cref="ParameterAttribute"/>), or derived from
/// property and column names.
/// </remarks>
/// <example>
/// <para>Example usage with a parameter attribute:</para>
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
///     ValueColumn = "DangIt"
/// };
/// 
/// SqlQuery query = procedureExecGenerator.Procedure(procedureExec);
/// ]]></code>
/// </example>
public class ParameterTag : IComparable<ParameterTag>, IEquatable<ParameterTag>, IEqualityComparer<ParameterTag>
{
    /// <summary>
    /// The base (core) parameter name. This value must not be <c>null</c>, empty, or whitespace.
    /// </summary>
    private readonly string _parameterBaseName;

    /// <summary>
    /// Optional prefix to prepend to the parameter name.
    /// </summary>
    private readonly string? _prefix;

    /// <summary>
    /// Optional prefix to prepend to the parameter name.
    /// </summary>
    private readonly string? _index;

    /// <summary>
    /// Represents the SQL type definition associated with this parameter, when known.
    /// </summary>
    /// <remarks>
    /// This value may be inferred from a runtime value or copied from the owning
    /// <see cref="ColumnInfo"/>. It is used when materializing <see cref="IDbDataParameter"/>
    /// instances for the generated SQL.
    /// </remarks>
    public SqlTypeDefinition? SqlType { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterTag"/> class.
    /// </summary>
    /// <param name="prefix">An optional prefix to prepend to the parameter name.</param>
    /// <param name="parameterName">The base parameter name. Must not be <c>null</c>, empty, or whitespace.</param>
    /// <param name="index">An optional index to append to the parameter name.</param>
    /// <exception cref="InvalidParameterIdentifierException">
    /// Thrown when <paramref name="parameterName"/> or the combined result is invalid per SQL identifier rules.
    /// </exception>
    internal ParameterTag(string? prefix, string parameterName, string? index, SqlTypeDefinition? sqlType)
    {
        if (SqlParameterPattern.Fails(parameterName))
            throw new InvalidParameterIdentifierException(parameterName);

        _parameterBaseName = parameterName;
        _prefix = prefix;
        _index = index;
        SqlType = sqlType;

        if (SqlParameterPattern.Fails(ToString()))
            throw new InvalidParameterIdentifierException(ToString());
    }

    /// <summary>
    /// Deeper copy constructor for the parameter tag.
    /// </summary>
    /// <param name="parameter"></param>
    private ParameterTag(ParameterTag parameter)
    {
        _parameterBaseName = parameter._parameterBaseName;
        _prefix = parameter._prefix;
        _index = parameter._index;
        SqlType = parameter.SqlType;
    }

    /// <summary>
    /// does necessary conversions of the object value. 
    /// </summary>
    /// <param name="value">the value</param>
    /// <returns>the converted value</returns>
    private static object ConvertValue(object? value)
    {
        if (value == null)
            return DBNull.Value;
        else if (value is XDocument xDocument)
            return xDocument.ToString();
        else if (value is XmlDocument xmlDocument)
            return ((object?)xmlDocument.OuterXml) ?? DBNull.Value; //the compiler didn't like xmlDocument.ToString() ?? DBNull.Value, so I had to get creative.
        else return value;
    }

    /// <summary>
    /// Creates a parameter key–value pair for the supplied value.
    /// </summary>
    /// <param name="value">The runtime value to bind to this parameter.</param>
    /// <returns>
    /// A <see cref="KeyValuePair{TKey, TValue}"/> whose key is a cloned
    /// <see cref="ParameterTag"/> with an inferred <see cref="SqlType"/> (when
    /// <paramref name="value"/> is not <c>null</c>), and whose value is the
    /// supplied value or <see cref="DBNull.Value"/>.
    /// </returns>
    internal KeyValuePair<ParameterTag, object> GetParameter(object? value)
    {
        ParameterTag parameterTag = new(this);
        parameterTag.SqlType ??= new(value);
        return new(parameterTag, ConvertValue(value));
    }

    /// <summary>
    /// Creates a parameter key–value pair for the supplied entity and column.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="column">
    /// The <see cref="ColumnInfo"/> used to locate the property and supply its <see cref="SqlTypeDefinition"/>.
    /// </param>
    /// <param name="entity">The entity instance to read the value from.</param>
    /// <returns>
    /// A <see cref="KeyValuePair{TKey, TValue}"/> whose key is a cloned
    /// <see cref="ParameterTag"/> configured with <paramref name="column"/>'s
    /// <see cref="ColumnInfo.SqlType"/>, and whose value is the property value
    /// or <see cref="DBNull.Value"/>.
    /// </returns>
    internal KeyValuePair<ParameterTag, object> GetParameter<T>(ColumnInfo column, T entity)
    {
        ParameterTag parameterTag = new(this);
        object? value = column.PropertyInfo.GetValue(entity);

        parameterTag.SqlType ??= column?.SqlType;
        return new(parameterTag, ConvertValue(value));
    }

    /// <summary>
    /// Creates a parameter key–value pair using an encrypted value for the given entity and column.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="encryption">The encryption service, or <c>null</c> to skip encryption.</param>
    /// <param name="column">Column metadata used to locate the property and SQL type.</param>
    /// <param name="entity">The entity instance providing the value.</param>
    /// <returns>
    /// A <see cref="KeyValuePair{TKey, TValue}"/> whose value is the encrypted string
    /// (or <c>null</c>/<see cref="DBNull.Value"/> when no encryption or value is available).
    /// </returns>
    internal KeyValuePair<ParameterTag, object> GetParameter<T>(IEncryption? encryption, ColumnInfo column, T entity) =>
        GetParameter(encryption?.Encrypt(column.PropertyInfo.GetValue(entity)?.ToString()));

    /// <summary>
    /// Implicitly converts a <see cref="ParameterTag"/> to its SQL string representation,
    /// combining prefix, base name, and index (if present) with underscores.
    /// </summary>
    /// <param name="value">The <see cref="ParameterTag"/> to convert.</param>
    /// <returns>
    /// The SQL parameter name string (e.g., <c>Prefix_Column_Index</c>; missing parts are omitted).
    /// </returns>
    public static implicit operator string(ParameterTag value)
    {
        IEnumerable<string?> stringParts = new List<string?>([value._prefix, value._parameterBaseName, value._index])
            .Where(part => part.IsNotNullOrWhiteSpace());
        return string.Join('_',  stringParts);
    }

    /// <summary>
    /// Returns the SQL string representation of this <see cref="ParameterTag"/> instance.
    /// </summary>
    /// <returns>
    /// The SQL parameter name string (e.g., <c>Prefix_Column_Index</c>; missing parts are omitted).
    /// </returns>
    public override string ToString() =>
        (string)this;

    /// <summary>
    /// Compares this <see cref="ParameterTag"/> with another instance to determine sort order.
    /// </summary>
    /// <param name="other">The other <see cref="ParameterTag"/> to compare.</param>
    /// <returns>
    /// A signed integer: <c>0</c> if equal; less than <c>0</c> if this instance precedes
    /// <paramref name="other"/>; greater than <c>0</c> if it follows.
    /// </returns>
    /// <remarks>
    /// Comparison is case-insensitive via <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// If <paramref name="other"/> is <c>null</c>, this instance is considered greater.
    /// </remarks>
    public int CompareTo(ParameterTag? other)
    {
        if (other is null) return 1;
        return string.Compare(this, (string)other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether this <see cref="ParameterTag"/> is equal to another instance.
    /// </summary>
    /// <param name="other">The other <see cref="ParameterTag"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if both represent the same parameter (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>Equality uses <see cref="StringComparison.OrdinalIgnoreCase"/>.</remarks>
    public bool Equals(ParameterTag? other)
    {
        if (other is null) return false;
        return string.Equals((string)this, (string)other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified object is equal to this <see cref="ParameterTag"/>.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="obj"/> is a <see cref="ParameterTag"/> representing the same
    /// parameter (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) =>
        obj is ParameterTag ct && Equals(ct);

    /// <summary>
    /// Returns a hash code for this <see cref="ParameterTag"/>, consistent with case-insensitive equality.
    /// </summary>
    /// <returns>
    /// An integer hash code computed from the string representation using
    /// <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </returns>
    public override int GetHashCode() =>
        ((string) this).GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Determines whether two <see cref="ParameterTag"/> instances are equal.
    /// </summary>
    /// <param name="x">The first <see cref="ParameterTag"/> to compare.</param>
    /// <param name="y">The second <see cref="ParameterTag"/> to compare.</param>
    /// <returns><c>true</c> if both represent the same parameter (case-insensitive); otherwise, <c>false</c>.</returns>
    public bool Equals(ParameterTag? x, ParameterTag? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="ParameterTag"/> instance,
    /// consistent with case-insensitive equality semantics.
    /// </summary>
    /// <param name="obj">The <see cref="ParameterTag"/> for which to compute a hash code.</param>
    /// <returns>An integer hash code for <paramref name="obj"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <c>null</c>.</exception>
    public int GetHashCode(ParameterTag obj) =>
        obj is null ? throw new ArgumentNullException(nameof(obj)) : obj.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="ParameterTag"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="ParameterTag"/> to compare.</param>
    /// <param name="right">The second <see cref="ParameterTag"/> to compare.</param>
    /// <returns><c>true</c> if both represent the same parameter (case-insensitive); otherwise, <c>false</c>.</returns>
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
    /// <returns><c>true</c> if the instances differ; otherwise, <c>false</c>.</returns>
    public static bool operator !=(ParameterTag? left, ParameterTag? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether this <see cref="ParameterTag"/> is empty.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the parameter string is <c>null</c>, empty, or whitespace; otherwise, <c>false</c>.
    /// </returns>
    internal bool IsEmpty() =>
        ((string)this).IsNullOrWhiteSpace();

    /// <summary>
    /// Creates a new <see cref="ParameterTag"/> with text prepended to the existing prefix (if any).
    /// </summary>
    /// <param name="textToPrepend">
    /// The text to prepend to the current prefix. If the prefix is empty, this becomes the new prefix.
    /// </param>
    /// <returns>A new <see cref="ParameterTag"/> with the updated prefix.</returns>
    internal ParameterTag PrefixPrepend(string? textToPrepend)
    {
        if (_prefix.IsNullOrWhiteSpace())
            return new(textToPrepend, _parameterBaseName, _index, SqlType);
        else
            return new($"{textToPrepend}_{_prefix}", _parameterBaseName, _index, SqlType);
    }

    /// <summary>
    /// Creates a new <see cref="ParameterTag"/> with the specified index appended.
    /// </summary>
    /// <param name="newIndex">The index value to append. Must not be <c>null</c>, empty, or whitespace.</param>
    /// <returns>A new <see cref="ParameterTag"/> that includes the specified index.</returns>
    /// <exception cref="ArgumentException">Thrown if an index has already been defined on this instance.</exception>
    internal ParameterTag AddIndex(string? newIndex)
    {
        if (_index.IsNullOrWhiteSpace())
            return new(_prefix, _parameterBaseName, newIndex, SqlType);
        else
            throw new ArgumentException("Index was already defined on the Parameter");
    }
}