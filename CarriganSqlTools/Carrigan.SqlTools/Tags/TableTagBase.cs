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

public abstract class TableTagBase : IEquatable<TableTagBase>, IEqualityOperators<TableTagBase, TableTagBase, bool>, ISqlFragment, IWhiteSpace, IEmpty
{

    /// <summary>
    /// Returns the schema-qualified table name represented by this tag.
    /// </summary>
    /// <returns>The table name, optionally qualified by its schema.</returns>
    public override abstract string ToString();

    /// <summary>
    /// Determines whether this instance identifies the same table as another <see cref="TableTagBase"/>.
    /// </summary>
    /// <param name="other">The other table tag to compare.</param>
    /// <returns><c>true</c> when the effective schema and table name are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(TableTagBase? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        else if (other is null)
            return false;

        else if (GetType() != other.GetType())
            return false;
        else if (this is TableTag thisTableTag && other is TableTag otherTableTag)
            return TableTag.Equals(thisTableTag, otherTableTag);

        else if (this is TableAliasTag thisTableAliasTag && other is TableAliasTag otherTableAliasTag)
            return TableAliasTag.Equals(thisTableAliasTag, otherTableAliasTag);
        else
            return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) =>
        Equals(obj as TableTagBase);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract override int GetHashCode();

    /// <summary>
    /// Determines whether two table tags identify the same table.
    /// </summary>
    public static bool operator ==(TableTagBase? left, TableTagBase? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two table tags identify different tables.
    /// </summary>
    public static bool operator !=(TableTagBase? left, TableTagBase? right) =>
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
    /// Indicates whether the rendered table tag is empty.
    /// </summary>
    public abstract bool IsEmpty();

    /// <summary>
    /// Indicates whether the rendered table tag is not empty.
    /// </summary>
    public bool IsNotEmpty() =>
        IsEmpty() == false;

    /// <summary>
    /// Flattens this fragment into the sequence of fragments used to render SQL text.
    /// </summary>
    /// <returns>A single-item sequence containing this table tag.</returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect)
    {
        yield return this;
    }
    /// <summary>
    /// Gets the SQL parameters contained by this fragment.
    /// </summary>
    /// <returns>An empty sequence because table-name fragments do not contain SQL parameters.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        [];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dialect"></param>
    /// <returns></returns>
    public abstract string ToSql(ISqlDialects dialect);
}
