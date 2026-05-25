using Carrigan.Core.DataTypes;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.RegularExpressions;

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
    internal ParameterTag(string baseName) :
        base(baseName, StringComparison.OrdinalIgnoreCase)
    {
        if (SqlParameterPattern.Fails(baseName))
            throw new InvalidParameterIdentifierException(baseName);
    }
}