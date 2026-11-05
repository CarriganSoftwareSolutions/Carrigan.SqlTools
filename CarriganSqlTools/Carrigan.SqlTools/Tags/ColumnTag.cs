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

public abstract class ColumnTag : IEquatable<ColumnTag>, IEqualityOperators<ColumnTag, ColumnTag, bool>, ISqlFragment, IWhiteSpace, IEmpty
{

    /// <summary>
    /// Returns the schema-qualified column name represented by this tag.
    /// </summary>
    /// <returns>The column name, optionally qualified by its schema.</returns>
    public override abstract string ToString();

    /// <summary>
    /// Determines whether this instance identifies the same column as another <see cref="ColumnTag"/>.
    /// </summary>
    /// <param name="other">The other column tag to compare.</param>
    /// <returns><c>true</c> when the effective schema and column name are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(ColumnTag? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        else if (other is null)
            return false;

        else if (GetType() != other.GetType())
            return false;
        else if (this is ColumnNameTag thisColumnTag && other is ColumnNameTag otherColumnTag)
            return ColumnNameTag.Equals(thisColumnTag, otherColumnTag);

        else if (this is ColumnAliasTag thisColumnAliasTag && other is ColumnAliasTag otherColumnAliasTag)
            return ColumnAliasTag.Equals(thisColumnAliasTag, otherColumnAliasTag);
        else
            return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) =>
        Equals(obj as ColumnTag);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract override int GetHashCode();

    /// <summary>
    /// Determines whether two column tags identify the same column.
    /// </summary>
    public static bool operator ==(ColumnTag? left, ColumnTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two column tags identify different tables.
    /// </summary>
    public static bool operator !=(ColumnTag? left, ColumnTag? right) =>
        !(left == right);

    /// <summary>
    /// 
    /// </summary>
    public abstract bool IsWhiteSpace();

    /// <summary>
    /// 
    /// </summary>
    public bool IsNotWhiteSpace() =>
        IsWhiteSpace() == false;

    /// <summary>
    /// Indicates whether the rendered column tag is empty.
    /// </summary>
    public abstract bool IsEmpty();

    /// <summary>
    /// Indicates whether the rendered column tag is not empty.
    /// </summary>
    public bool IsNotEmpty() =>
        IsEmpty() == false;

    /// <summary>
    /// Flattens this fragment into the sequence of fragments used to render SQL text.
    /// </summary>
    /// <returns>A single-item sequence containing this column tag.</returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect)
    {
        yield return this;
    }
    /// <summary>
    /// Gets the SQL parameters contained by this fragment.
    /// </summary>
    /// <returns>An empty sequence because column-name fragments do not contain SQL parameters.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        [];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dialect"></param>
    /// <returns></returns>
    public abstract string ToSql(ISqlDialects dialect);
}
