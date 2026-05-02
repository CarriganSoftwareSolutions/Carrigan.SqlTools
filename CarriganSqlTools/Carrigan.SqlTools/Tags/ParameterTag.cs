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
    internal readonly string BaseName;

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
    /// <param name="baseName">The base parameter name. Must not be <c>null</c>, empty, or whitespace.</param>
    /// <param name="sqlType">The optional SQL type definition associated with this parameter.</param>
    /// 
    /// 
    /// <exception cref="InvalidParameterIdentifierException">
    /// Thrown when <paramref name="baseName"/> or the combined result is invalid per SQL identifier rules.
    /// </exception>
    internal ParameterTag(string baseName, SqlTypeDefinition? sqlType) :
        base(baseName, StringComparison.OrdinalIgnoreCase)
    {
        if (SqlParameterPattern.Fails(baseName))
            throw new InvalidParameterIdentifierException(baseName);

        BaseName = baseName;
        SqlType = sqlType;

        if (SqlParameterPattern.Fails(ToString()))
            throw new InvalidParameterIdentifierException(ToString());
    }

    /// <summary>
    /// Deeper copy constructor for the parameter tag.
    /// </summary>
    /// <param name="parameter">The parameter tag to clone.</param>
    internal ParameterTag(ParameterTag parameter, object? value)
        : this(parameter?.BaseName ?? throw new InvalidParameterIdentifierException("null"), parameter.SqlType ?? new SqlTypeDefinition(value))
    {
    }
}