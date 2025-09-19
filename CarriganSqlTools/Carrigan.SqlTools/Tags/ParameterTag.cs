using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// This class represents the Parameter "Tag", or identifier.
/// </summary>
public class ParameterTag : IComparable<ParameterTag>, IEquatable<ParameterTag>, IEqualityComparer<ParameterTag>
{
    private readonly string _parameterBaseName;
    private readonly string? _prefix;
    private readonly string? _index;

    internal ParameterTag(string? prefix, string parameterName, string? index)
    {
        if (parameterName.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(parameterName), parameterName);

        _parameterBaseName = parameterName;
        _prefix = prefix;
        _index = index;
    }

    public static implicit operator string(ParameterTag value)
    {
        IEnumerable<string?> stringParts = new List<string?>([value._prefix, value._parameterBaseName, value._index])
            .Where(part => part.IsNotNullOrWhiteSpace());
        return string.Join('_',  stringParts);
    }

    public override string ToString() =>
        (string)this;

    public int CompareTo(ParameterTag? other)
    {
        if (other is null) return 1;
        return string.Compare((string)this, (string)other, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(ParameterTag? other)
    {
        if (other is null) return false;
        return string.Equals((string)this, (string)other, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) =>
        obj is ParameterTag ct && Equals(ct);

    public override int GetHashCode() =>
        ((string) this).GetHashCode(StringComparison.OrdinalIgnoreCase);

    public bool Equals(ParameterTag? x, ParameterTag? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    public int GetHashCode(ParameterTag obj) =>
        obj is null ? throw new ArgumentNullException(nameof(obj)) : obj.GetHashCode();

    public static bool operator ==(ParameterTag? left, ParameterTag? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(ParameterTag? left, ParameterTag? right)
    {
        return !(left == right);
    }

    public bool IsEmpty() =>
        ((string)this).IsNullOrWhiteSpace();

    public ParameterTag PrefixPrepend(string? textToAppend)
    {
        if (_prefix.IsNullOrWhiteSpace())
            return new(textToAppend, _parameterBaseName, _index);
        else
            return new($"{textToAppend}_{_prefix}", _parameterBaseName, _index);
    }

    public ParameterTag PrefixAppend(string? textToAppend)
    {
        if (_prefix.IsNullOrWhiteSpace())
            return new(textToAppend, _parameterBaseName, _index);
        else
            return new($"{_prefix}_{textToAppend}", _parameterBaseName, _index);
    }

    public ParameterTag AddIndex(string? newIndex)
    {
        if (_index.IsNullOrWhiteSpace())
            return new(_prefix, _parameterBaseName, newIndex);
        else
            throw new ArgumentException("Index was already defined on the Parameter");
    }
}