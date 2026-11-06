using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using System.Numerics;

namespace Carrigan.SqlTools.Tags;

public class TableAliasTag : TableTag, IEquatable<TableAliasTag>, IEqualityOperators<TableAliasTag, TableAliasTag, bool>, ISqlFragment, IWhiteSpace, IEmpty
{
    /// <summary>
    /// 
    /// </summary>
    internal readonly TableAlias TableAlias;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="tableAlias"></param>
    internal TableAliasTag(TableAlias tableAlias) =>
        TableAlias = tableAlias;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aliasName"></param>
    [ExternalOnly]//An external only marked as internal can still be used by the unit tests class.
    internal TableAliasTag(string aliasName) :this (new TableAlias(aliasName))
    {
    }

    /// <summary>
    /// Creates a new TableAliasTag instance or returns null if the tableAlias is null.
    /// </summary>
    /// <param name="tableAlias">
    /// The table alias to wrap in a TableAliasTag.
    /// </param>
    /// <returns>A new TableAliasTag instance or null if the tableAlias is null.</returns>
    [ExternalOnly]
    public static TableAliasTag? New(string? tableAlias) =>
        New(TableAlias.New(tableAlias));

    /// <summary>
    /// Creates a new TableAliasTag instance or returns null if the tableAlias is null.
    /// </summary>
    /// <param name="tableAlias">
    /// The table alias to wrap in a TableAliasTag.
    /// </param>
    /// <returns>A new TableAliasTag instance or null if the tableAlias is null.</returns>
    public static TableAliasTag? New(TableAlias? tableAlias) =>
        tableAlias is null ? null : new TableAliasTag(tableAlias);


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        TableAlias.ToString();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(TableAliasTag? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        return TableAlias.Equals(other.TableAlias);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) =>
        Equals(obj as TableAliasTag);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() =>
        HashCode.Combine(TableAlias);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(TableAliasTag? left, TableAliasTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool operator !=(TableAliasTag? left, TableAliasTag? right) =>
        !(left == right);

    /// <summary>
    /// Indicates whether the rendered table tag is empty or consists only of whitespace characters.
    /// </summary>
    public override bool IsWhiteSpace() =>
        TableAlias.IsWhiteSpace();

    /// <summary>
    /// Indicates whether the rendered table tag is empty.
    /// </summary>
    public override bool IsEmpty() =>
        TableAlias.IsEmpty();

    /// <summary>
    /// Renders the schema-qualified table name using the supplied SQL dialect.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to quote and combine the schema and table identifiers.</param>
    /// <returns>The rendered table identifier.</returns>
    public override string ToSql(ISqlDialects dialect) =>
        dialect.QuoteIdentifier(TableAlias.ToString());
}
