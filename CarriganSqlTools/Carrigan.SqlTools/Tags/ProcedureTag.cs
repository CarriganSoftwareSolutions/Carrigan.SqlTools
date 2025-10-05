using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.RegularExpressions;
using System.Reflection;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a stored procedure identifier (“tag”) in the form <c>[Schema].[Procedure]</c>.
/// The <c>[Schema]</c> segment is included only if explicitly provided. Implements
/// comparison and equality for use in sorting and hashed collections.
/// </summary>
/// <example>
/// <para>
///  Using Procedure
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
public class ProcedureTag : IComparable<ProcedureTag>, IEquatable<ProcedureTag>, IEqualityComparer<ProcedureTag>
{
    private readonly string _procedureTag;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcedureTag"/> class.
    /// </summary>
    /// <param name="schemaName">The optional schema name. If <c>null</c> or empty, only the procedure name is used.</param>
    /// <param name="procedureName">The procedure name. Must not be <c>null</c> or empty.</param>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when <paramref name="procedureName"/> or a non-empty <paramref name="schemaName"/> fails SQL identifier validation.
    /// </exception>
    internal ProcedureTag(string? schemaName, string procedureName) => 
        _procedureTag = schemaName.IsNullOrEmpty() ? $"[{procedureName}]" : $"[{schemaName}].[{procedureName}]";

    /// <summary>
    /// Implicitly converts a <see cref="ProcedureTag"/> to its SQL string representation,
    /// e.g., <c>[Schema].[Procedure]</c> or <c>[Procedure]</c>.
    /// </summary>
    /// <param name="tag">The <see cref="ProcedureTag"/> to convert.</param>
    /// <returns>The SQL-formatted procedure identifier string.</returns>
    public static implicit operator string(ProcedureTag tag) =>
        tag._procedureTag;

    /// <summary>
    /// Returns the SQL string representation of this <see cref="ProcedureTag"/> instance.
    /// </summary>
    /// <returns>The SQL-formatted procedure identifier string.</returns>
    public override string ToString() =>
        _procedureTag;

    /// <summary>
    /// Compares this instance to another <see cref="ProcedureTag"/> and returns a value
    /// indicating the sort order.
    /// </summary>
    /// <param name="other">The other <see cref="ProcedureTag"/> to compare.</param>
    /// <returns>
    /// A signed integer indicating relative order: 0 if equal; less than 0 if this instance
    /// precedes <paramref name="other"/>; greater than 0 if it follows.
    /// </returns>
    /// <remarks>Comparison is case-sensitive and uses <see cref="StringComparison.Ordinal"/>.</remarks>
    public int CompareTo(ProcedureTag? other)
    {
        if (other is null) return 1; 
        return string.Compare(_procedureTag, other._procedureTag, StringComparison.Ordinal);
    }
    /// <summary>
    /// Determines whether this <see cref="ProcedureTag"/> is equal to another instance.
    /// </summary>
    /// <param name="other">The other <see cref="ProcedureTag"/> to compare.</param>
    /// <returns><c>true</c> if both represent the same identifier; otherwise, <c>false</c>.</returns>
    /// <remarks>Equality is case-sensitive and uses <see cref="StringComparison.Ordinal"/>.</remarks>
    public bool Equals(ProcedureTag? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(_procedureTag, other._procedureTag, StringComparison.Ordinal);
    }
    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="ProcedureTag"/> equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        Equals(obj as ProcedureTag);

    /// <summary>
    /// Returns a hash code for this <see cref="ProcedureTag"/> instance.
    /// </summary>
    /// <returns>An integer hash code computed using <see cref="StringComparison.Ordinal"/>.</returns>
    public override int GetHashCode() =>
        _procedureTag.GetHashCode(StringComparison.Ordinal);

    /// <summary>
    /// Determines whether two <see cref="ProcedureTag"/> instances are equal.
    /// </summary>
    /// <param name="x">The first <see cref="ProcedureTag"/> to compare.</param>
    /// <param name="y">The second <see cref="ProcedureTag"/> to compare.</param>
    /// <returns><c>true</c> if both are equal; otherwise, <c>false</c>.</returns>
    /// <remarks>Equality is case-sensitive and uses <see cref="StringComparison.Ordinal"/>.</remarks>
    public bool Equals(ProcedureTag? x, ProcedureTag? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return string.Equals(x._procedureTag, y._procedureTag, StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="ProcedureTag"/> instance.
    /// </summary>
    /// <param name="obj">The <see cref="ProcedureTag"/> for which to compute a hash code.</param>
    /// <returns>An integer hash code computed using <see cref="StringComparison.Ordinal"/>.</returns>
    public int GetHashCode(ProcedureTag obj) =>
        obj._procedureTag.GetHashCode(StringComparison.Ordinal);

    /// <summary>
    /// Determines whether two <see cref="ProcedureTag"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="ProcedureTag"/> to compare.</param>
    /// <param name="right">The second <see cref="ProcedureTag"/> to compare.</param>
    /// <returns><c>true</c> if both represent the same identifier; otherwise, <c>false</c>.</returns>
    /// <remarks>Equivalent to <see cref="Equals(ProcedureTag?, ProcedureTag?)"/>.</remarks>
    public static bool operator ==(ProcedureTag? left, ProcedureTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="ProcedureTag"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="ProcedureTag"/> to compare.</param>
    /// <param name="right">The second <see cref="ProcedureTag"/> to compare.</param>
    /// <returns><c>true</c> if they differ; otherwise, <c>false</c>.</returns>
    public static bool operator !=(ProcedureTag? left, ProcedureTag? right) =>
        !(left == right);

    /// <summary>
    /// Retrieves the <see cref="ProcedureTag"/> associated with the specified entity type by using
    /// the internal SQL tools reflection cache.
    /// </summary>
    /// <param name="value">The entity type for which to retrieve the procedure tag.</param>
    /// <returns>The <see cref="ProcedureTag"/> for the specified type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the underlying reflection cache does not expose a non-public static
    /// <c>ProcedureTag</c> property, or when it returns <c>null</c>.
    /// </exception>
    public static ProcedureTag Get(Type value)
    {
        ArgumentNullException.ThrowIfNull(value);

        // Construct the generic type: SqlToolsReflectorCache<value>
        Type cacheType = typeof(SqlToolsReflectorCache<>).MakeGenericType(value);

        // Get the static property 'TableTag' on the constructed type.
        PropertyInfo tableTagProperty = cacheType.GetProperty("ProcedureTag", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException($"The property 'ProcedureTag' was not found on type '{cacheType.FullName}'.");

        // Retrieve the value of the TableTag property.
        return (ProcedureTag?)tableTagProperty.GetValue(null) ?? throw new InvalidOperationException($"The property 'TableTag' on type '{cacheType.FullName}' returned null.");
    }
}

