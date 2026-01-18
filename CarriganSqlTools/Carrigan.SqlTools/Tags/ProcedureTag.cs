using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System.Reflection;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a stored procedure identifier (“tag”) in the form <c>[Schema].[Procedure]</c>.
/// The <c>[Schema]</c> segment is included only when explicitly provided.
/// </summary>
/// <remarks>
/// This type uses <see cref="StringWrapper"/> to provide consistent equality, ordering,
/// and hashing semantics (case-sensitive via <see cref="StringComparison.Ordinal"/>).
/// <para>
/// Note: Inherited equality and ordering operations can throw <see cref="InvalidOperationException"/>
/// if this instance is compared against a different <see cref="StringWrapper"/> that uses a different
/// <see cref="StringComparison"/> mode.
/// </para>
/// <para>
/// SQL identifier correctness (invalid characters, reserved words, length constraints, etc.)
/// is validated by the SQL generator.
/// </para>
/// </remarks>
/// <example>
/// <para>Using Procedure</para>
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
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// [schema].[UpdateThing]
/// ]]></code>
/// </example>
internal class ProcedureTag : StringWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProcedureTag"/> class.
    /// </summary>
    /// <param name="schemaName">
    /// The optional schema name. If <c>null</c> or empty, only the procedure name is used.
    /// </param>
    /// <param name="procedureName">
    /// The procedure name.
    /// </param>
    internal ProcedureTag(SchemaName? schemaName, ProcedureName procedureName)
        : base(CreateProcedureTagString(schemaName, procedureName), StringComparison.Ordinal)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcedureTag"/> class.
    /// </summary>
    /// <remarks>
    /// Marked <see cref="ExternalOnlyAttribute"/> so unit tests can access it while keeping the API internal.
    /// </remarks>
    /// <param name="schemaName">
    /// The optional schema name. If <c>null</c> or empty, only the procedure name is used.
    /// </param>
    /// <param name="procedureName">The procedure name.</param>
    [ExternalOnly]
    internal ProcedureTag(string? schemaName, string procedureName)
        : this(SchemaName.New(schemaName), new(procedureName))
    {
    }

    /// <summary>
    /// Retrieves the <see cref="ProcedureTag"/> associated with the specified entity type by using
    /// the internal SQL Tools reflection cache.
    /// </summary>
    /// <param name="value">The entity CLR <see cref="Type"/> whose procedure tag should be retrieved.</param>
    /// <returns>The <see cref="ProcedureTag"/> for the specified type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="AmbiguousMatchException">Thrown when multiple matching properties are found.</exception>
    /// <exception cref="TargetInvocationException">Thrown when the property getter throws an exception.</exception>
    /// <exception cref="MethodAccessException">Thrown when the property getter is inaccessible.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the underlying reflection cache does not expose a non-public static
    /// <c>ProcedureTag</c> property, or when the property value is <c>null</c>.
    /// </exception>
    internal static ProcedureTag Get(Type value)
    {
        ArgumentNullException.ThrowIfNull(value);

        Type cacheType = typeof(SqlToolsReflectorCache<>).MakeGenericType(value);

        PropertyInfo procedureTagProperty =
            cacheType.GetProperty("ProcedureTag", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException($"The property 'ProcedureTag' was not found on type '{cacheType.FullName}'.");

        return (ProcedureTag?)procedureTagProperty.GetValue(null)
            ?? throw new InvalidOperationException($"The property 'ProcedureTag' on type '{cacheType.FullName}' returned null.");
    }

    private static string CreateProcedureTagString(SchemaName? schemaName, ProcedureName procedureName)
    {
        if (schemaName.IsNullOrEmpty())
            return $"[{procedureName}]";
        else
            return $"[{schemaName}].[{procedureName}]";
    }
}
