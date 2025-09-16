using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using System.Reflection;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// This class represents the Table "Tag", or identifier. IE [Schema].[Table]
/// [Schema] is only included if you define the schema of the table.
/// Aside from various comparison and equality interfaces, this class is locked down to internal.
/// </summary>
public class ProcedureTag : IComparable<ProcedureTag>, IEquatable<ProcedureTag>, IEqualityComparer<ProcedureTag>
{
    private readonly string _procedureTag;

    internal ProcedureTag(string? schemaName, string procedureName)
    {
        if (procedureName.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(procedureName), $"{nameof(procedureName)} requires a value.");
        else
            _procedureTag = schemaName.IsNullOrEmpty() ? $"[{procedureName}]" : $"[{schemaName}].[{procedureName}]";

        if (SqlIdentifierPattern.Fails(procedureName))
            throw new SqlNamePatternException(this);
        if(schemaName.IsNotNullOrWhiteSpace() && SqlIdentifierPattern.Fails(schemaName))
            throw new SqlNamePatternException(this);
    }

    public static implicit operator string(ProcedureTag tag) =>
        tag._procedureTag;

    public override string ToString() =>
        _procedureTag;

    public int CompareTo(ProcedureTag? other)
    {
        if (other is null) return 1; 
        return string.Compare(_procedureTag, other._procedureTag, StringComparison.Ordinal);
    }

    public bool Equals(ProcedureTag? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(_procedureTag, other._procedureTag, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) =>
        Equals(obj as ProcedureTag);

    public override int GetHashCode() =>
        _procedureTag.GetHashCode(StringComparison.Ordinal);

    public bool Equals(ProcedureTag? x, ProcedureTag? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return string.Equals(x._procedureTag, y._procedureTag, StringComparison.Ordinal);
    }

    public int GetHashCode(ProcedureTag obj) =>
        obj._procedureTag.GetHashCode(StringComparison.Ordinal);

    public static bool operator ==(ProcedureTag? left, ProcedureTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ProcedureTag? left, ProcedureTag? right) =>
        !(left == right);

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

