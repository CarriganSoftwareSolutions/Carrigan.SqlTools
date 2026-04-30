using Carrigan.Core.DataTypes;
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
/// <para>
/// This class implements <see cref="IComparable{T}"/>, <see cref="IEquatable{T}"/>,
/// and <see cref="IEqualityComparer{T}"/> to support sorting, equality checks,
/// and use in collections that rely on hashing. Parameter names may be provided
/// explicitly (for example, via <see cref="ParameterAttribute"/>), or derived from
/// property and column names.
/// </para>
/// <para>
/// Note: Inherited equality and ordering operations can throw <see cref="InvalidOperationException"/>
/// if this instance is compared against a different <see cref="StringWrapper"/> that uses a different
/// <see cref="StringComparison"/> mode.
/// </para>
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
public class ParameterTag : StringWrapper
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
    /// Optional index to append to the parameter name.
    /// </summary>
    private readonly string? _index;

    /// <summary>
    /// Represents the SQL type definition associated with this parameter, when known.
    /// </summary>
    /// <remarks>
    /// This value may be inferred from a runtime value (including <c>null</c>, which infers <see cref="SqlDbType.Variant"/>),
    /// or copied from the owning <see cref="ColumnInfo"/>. It is used when materializing <see cref="IDbDataParameter"/>
    /// instances for the generated SQL.
    /// </remarks>
    public SqlTypeDefinition? SqlType { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterTag"/> class.
    /// </summary>
    /// <param name="prefix">An optional prefix to prepend to the parameter name.</param>
    /// <param name="parameterName">The base parameter name. Must not be <c>null</c>, empty, or whitespace.</param>
    /// <param name="index">An optional index to append to the parameter name.</param>
    /// <param name="sqlType">The optional SQL type definition associated with this parameter.</param>
    /// <exception cref="InvalidParameterIdentifierException">
    /// Thrown when <paramref name="parameterName"/> or the combined result is invalid per SQL identifier rules.
    /// </exception>
    internal ParameterTag(string? prefix, string parameterName, string? index, SqlTypeDefinition? sqlType) :
        base(CreateParameterTagString(prefix, parameterName, index), StringComparison.OrdinalIgnoreCase)
    {
        //TODO: Determine if these sql pattern checks are redundant
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
    /// <param name="parameter">The parameter tag to clone.</param>
    private ParameterTag(ParameterTag parameter) :
        base(CreateParameterTagString(parameter._prefix, parameter._parameterBaseName, parameter._index), StringComparison.OrdinalIgnoreCase)
    {
        _parameterBaseName = parameter._parameterBaseName;
        _prefix = parameter._prefix;
        _index = parameter._index;
        SqlType = parameter.SqlType;
    }

    /// <summary>
    /// Deeper copy constructor for the parameter tag.
    /// </summary>
    /// <param name="parameter">The parameter tag to clone.</param>
    internal ParameterTag(ParameterTag parameter, object? value) :
        base(CreateParameterTagString(parameter._prefix, parameter._parameterBaseName, parameter._index), StringComparison.OrdinalIgnoreCase)
    {
        _parameterBaseName = parameter._parameterBaseName;
        _prefix = parameter._prefix;
        _index = parameter._index;
        SqlType = parameter.SqlType ?? new(value);
    }

    /// <summary>
    /// Performs the necessary conversions for a parameter value
    /// before it is passed to the database.
    /// </summary>
    /// <param name="value">
    /// The value to convert. A <c>null</c> value is converted to
    /// <see cref="DBNull.Value"/>.
    /// </param>
    /// <returns>
    /// The converted value suitable for database operations.
    /// </returns>
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
    /// the current instance does not already have one), and whose value is the
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

        parameterTag.SqlType ??= column.SqlType;
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
    /// Creates a new <see cref="ParameterTag"/> with text prepended to the existing prefix (if any).
    /// </summary>
    /// <param name="textToPrepend">
    /// The text to prepend to the current prefix. If the existing prefix is empty, this becomes the new prefix.
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
    /// <param name="newIndex">The index value to append. If <c>null</c> or whitespace, no index is appended.</param>
    /// <returns>A new <see cref="ParameterTag"/> that includes the specified index.</returns>
    /// <exception cref="ArgumentException">Thrown if an index has already been defined on this instance.</exception>
    internal ParameterTag AddIndex(string? newIndex)
    {
        if (_index.IsNullOrWhiteSpace())
            return new(_prefix, _parameterBaseName, newIndex, SqlType);
        else
            throw new ArgumentException("Index was already defined on the Parameter", nameof(newIndex));
    }

    /// <summary>
    /// Creates the parameter tag string by combining the prefix, parameter name, and index with underscores.
    /// </summary>
    /// <param name="prefix">The prefix to prepend to the parameter name.</param>
    /// <param name="parameterName">The base name of the parameter.</param>
    /// <param name="index">The index to append to the parameter name.</param>
    /// <returns>The constructed parameter tag string.</returns>
    /// <exception cref="InvalidParameterIdentifierException"></exception>
    private static string CreateParameterTagString(string? prefix, string parameterName, string? index)
    {
        if (SqlParameterPattern.Fails(parameterName))
            throw new InvalidParameterIdentifierException(parameterName);

        IEnumerable<string?> parts =
        [
            prefix,
            parameterName,
            index
        ];

        IEnumerable<string?> nonEmptyParts =
            parts.Where(static part => part.IsNotNullOrWhiteSpace());

        string parameterTag =
            string.Join('_', nonEmptyParts);

        if (SqlParameterPattern.Fails(parameterTag))
            throw new InvalidParameterIdentifierException(parameterTag);

        return parameterTag;
    }
}