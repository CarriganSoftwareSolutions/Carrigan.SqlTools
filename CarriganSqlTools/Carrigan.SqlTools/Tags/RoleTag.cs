using Carrigan.SqlTools;
using Carrigan.SqlTools.Exceptions;

namespace Carrigan.SqlTools.Tags;

public class RoleTag : IComparable<RoleTag>, IEquatable<RoleTag>, IEqualityComparer<RoleTag>
{

    private readonly string _role;
    public RoleTag(string role)
    {
        _role = role;
        if (SqlIdentifierPattern.Fails(role))
            throw new SqlNamePatternException(this);
    }
    public static implicit operator string(RoleTag role) =>
        role._role;

    public override string ToString() =>
        _role;

    public int CompareTo(RoleTag? other)
    {
        if (other is null) return 1;
        return string.Compare(_role, other._role, StringComparison.Ordinal);
    }

    public bool Equals(RoleTag? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(_role, other._role, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as RoleTag);
    }

    public override int GetHashCode()
    {
        return _role.GetHashCode(StringComparison.Ordinal);
    }

    public bool Equals(RoleTag? x, RoleTag? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return string.Equals(x._role, y._role, StringComparison.Ordinal);
    }

    public int GetHashCode(RoleTag obj)
    {
        return obj._role.GetHashCode(StringComparison.Ordinal);
    }
    public static bool operator ==(RoleTag? left, RoleTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(RoleTag? left, RoleTag? right) => !(left == right);
}
