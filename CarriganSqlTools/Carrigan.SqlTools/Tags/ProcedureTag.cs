using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System.Numerics;
using System.Reflection;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a stored procedure identifier (“tag”) in the form <c>[Schema].[Procedure]</c>.
/// The <c>[Schema]</c> segment is included only when explicitly provided.
/// </summary>
/// <remarks>
/// Equality and hashing are based on the schema and procedure-name components using their
/// case-sensitive identifier semantics. An empty schema is treated the same as no schema.
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
/// --PostgreSql
/// "schema"."UpdateThing"
/// 
/// --SqlServer
/// [schema].[UpdateThing]
/// ]]></code>
/// </example>
public class ProcedureTag : IEquatable<ProcedureTag>, IEqualityOperators<ProcedureTag, ProcedureTag, bool>, ISqlFragment, IWhiteSpace, IEmpty
{
    /// <summary>
    /// The optional schema that qualifies the stored procedure name.
    /// </summary>
    internal readonly SchemaName? SchemaName;

    /// <summary>
    /// The stored procedure identifier without the schema qualifier.
    /// </summary>
    internal readonly ProcedureName ProcedureName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcedureTag"/> class.
    /// </summary>
    /// <param name="schemaName">
    /// The optional schema name. If <c>null</c> or empty, only the procedure name is used.
    /// </param>
    /// <param name="procedureName">The procedure name.</param>
    internal ProcedureTag(SchemaName? schemaName, ProcedureName procedureName)
    {
        ArgumentNullException.ThrowIfNull(procedureName);

        SchemaName = schemaName;
        ProcedureName = procedureName;
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

    /// <summary>
    /// Returns the schema-qualified procedure name represented by this tag.
    /// </summary>
    /// <returns>The procedure name, optionally qualified by its schema.</returns>
    public override string ToString() =>
        SchemaName.IsNotNullOrEmpty() ? $"{SchemaName}.{ProcedureName}" : ProcedureName.ToString();

    /// <summary>
    /// Determines whether this instance identifies the same procedure as another <see cref="ProcedureTag"/>.
    /// </summary>
    /// <param name="other">The other procedure tag to compare.</param>
    /// <returns><c>true</c> when the effective schema and procedure name are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(ProcedureTag? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        if (SchemaName.IsNotNullOrEmpty() != other.SchemaName.IsNotNullOrEmpty())
            return false;

        if (SchemaName.IsNotNullOrEmpty() && !SchemaName.Equals(other.SchemaName))
            return false;

        return ProcedureName.Equals(other.ProcedureName);
    }

    /// <summary>
    /// Determines whether the specified object identifies the same procedure as this instance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> when <paramref name="obj"/> is an equivalent <see cref="ProcedureTag"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        Equals(obj as ProcedureTag);

    /// <summary>
    /// Returns a hash code for this procedure tag.
    /// </summary>
    /// <returns>A hash code consistent with <see cref="Equals(ProcedureTag?)"/>.</returns>
    public override int GetHashCode()
    {
        SchemaName? schemaName = SchemaName.IsNotNullOrEmpty() ? SchemaName : null;
        return HashCode.Combine(schemaName, ProcedureName);
    }

    /// <summary>
    /// Determines whether two procedure tags identify the same procedure.
    /// </summary>
    public static bool operator ==(ProcedureTag? left, ProcedureTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two procedure tags identify different procedures.
    /// </summary>
    public static bool operator !=(ProcedureTag? left, ProcedureTag? right) =>
        !(left == right);

    /// <summary>
    /// Indicates whether the rendered procedure tag is empty or consists only of whitespace characters.
    /// </summary>
    public bool IsWhiteSpace() =>
        SchemaName.IsNullOrWhiteSpace() && ProcedureName.IsWhiteSpace();

    /// <summary>
    /// Indicates whether the rendered procedure tag contains at least one non-whitespace character.
    /// </summary>
    public bool IsNotWhiteSpace() =>
        IsWhiteSpace() == false;

    /// <summary>
    /// Indicates whether the rendered procedure tag is empty.
    /// </summary>
    public bool IsEmpty() =>
        SchemaName.IsNullOrEmpty() && ProcedureName.IsEmpty();

    /// <summary>
    /// Indicates whether the rendered procedure tag is not empty.
    /// </summary>
    public bool IsNotEmpty() =>
        IsEmpty() == false;

    /// <summary>
    /// Flattens this fragment into the sequence of fragments used to render SQL text.
    /// </summary>
    /// <returns>A single-item sequence containing this procedure tag.</returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect)
    {
        yield return this;
    }

    /// <summary>
    /// Gets the SQL parameters contained by this fragment.
    /// </summary>
    /// <returns>An empty sequence because procedure-name fragments do not contain SQL parameters.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        [];

    /// <summary>
    /// Renders the SQL fragment using the supplied dialect.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render the fragment.</param>
    /// <returns>The rendered procedure identifier.</returns>
    public string ToSql(ISqlDialects dialect) =>
        dialect.RenderProcedureTag(this);
}
