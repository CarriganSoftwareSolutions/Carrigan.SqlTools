using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.IdentifierTypes;
using System.Numerics;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a SELECT projection tag for a single SQL expression, consisting of the expression
/// and an optional alias.
/// </summary>
public class SelectTag : IEquatable<SelectTag>, IEqualityOperators<SelectTag, SelectTag, bool>,  ISqlFragment
{
    /// <summary>
    /// The SQL expression projected by this select item.
    /// </summary>
    public SqlExpression SqlExpression { get; }

    /// <summary>
    /// The optional alias applied to this select item.
    /// </summary>
    internal readonly AliasTag? AliasTag;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class.
    /// </summary>
    /// <param name="columnTag">The column identifier to select.</param>
    /// <param name="aliasTag">The optional alias to apply to the selected column.</param>
    [Obsolete("No longer needed, except by public obsolete methods.")]
    internal SelectTag(ColumnTag columnTag, AliasTag? aliasTag = null)
        : this(new ColumnTagExpression(columnTag), aliasTag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using the provided SQL expression and optional alias name.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to project.</param>
    /// <param name="aliasName">An optional alias to use for this projection.</param>
    public SelectTag(SqlExpression sqlExpression, AliasName aliasName) : this(sqlExpression, AliasTag.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using the provided SQL expression and optional alias name.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to project.</param>
    /// <param name="aliasName">An optional alias to use for this projection.</param>
    [ExternalOnly]
    public SelectTag(SqlExpression sqlExpression, string aliasName) : this(sqlExpression, new AliasName(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to select.</param>
    /// <param name="aliasTag">The optional alias to apply to the selected expression.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpression"/> is <c>null</c>.
    /// </exception>
    public SelectTag(SqlExpression sqlExpression, AliasTag? aliasTag = null)
    {
        ArgumentNullException.ThrowIfNull(sqlExpression, nameof(sqlExpression));

        SqlExpression = sqlExpression;
        AliasTag = aliasTag;
    }

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and the specified tag.
    /// </summary>
    /// <param name="selectTag2">The select tag to append.</param>
    /// <returns>A new <see cref="SelectTags"/> collection containing both tags.</returns>
    public SelectTags Append(SelectTag selectTag2) =>
        new(this, selectTag2);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and the supplied tags.
    /// </summary>
    /// <param name="selectTags">The select tags to append.</param>
    public SelectTags Concat(SelectTags selectTags) =>
        new SelectTags(this).Concat(selectTags._selectTags);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and the supplied tags.
    /// </summary>
    /// <param name="selectTags">The select tags to append.</param>
    public SelectTags Concat(params IEnumerable<SelectTag> selectTags) =>
        new SelectTags(this).Concat(selectTags);

    //TODO: Validate that is being used correctly.
    /// <summary>
    /// Gets the simple column tag represented by a column-shaped expression, when applicable.
    /// </summary>
    private static ColumnTag? GetSimpleColumnTag(SqlExpression sqlExpression) =>
        sqlExpression switch
        {
            IColumnBase column => column.ColumnInfo.ColumnTag,
            ColumnTagExpression columnTagExpression => columnTagExpression.ColumnTag,
            _ => null
        };

    /// <summary>
    /// Gets the table tags that participate in this select expression.
    /// </summary>
    internal IEnumerable<TableTag> TableTags =>
        SqlExpression.AllParticipatingTables;

    internal IEnumerable<SqlExpression> NonAggregateColumnExpressions =>
        SqlExpression.AllNonAggregateColumns;

    /// <summary>
    /// Gets the expected result set column name for this projection, choosing the alias
    /// when present, the underlying column name for simple columns, or the expression text otherwise.
    /// </summary>
    //TODO: Validate that is being used correctly.
    internal ResultColumnName ResultColumnName =>
        new(AliasTag?.ToString() ?? GetSimpleColumnTag(SqlExpression)?.ColumnName.ToString() ?? SqlExpression.ToString());

    /// <summary>
    /// Indicates whether this select item is valid in an aggregate SELECT list for the supplied <c>GROUP BY</c> clause.
    /// </summary>
    /// <returns>The aggregate status of the underlying expression or column.</returns>
    [Obsolete("No longer used, HasAggregates is more useful")]
    public bool IsAggregate() =>
        SqlExpression.IsAggregate();

    public bool HasAggregates() =>
        SqlExpression.HasAggregates();

    /// <summary>
    /// Indicates whether this select item projects one or more columns.
    /// </summary>
    /// <returns>
    /// <c>true</c> if this select item projects one or more columns; otherwise, <c>false</c>.
    /// </returns>
    public bool HasColumns() =>
        SqlExpression.HasColumns();

    /// <summary>
    /// Flattens this fragment into the sequence of fragments used to render SQL text.
    /// </summary>
    /// <returns>A flattened sequence of SQL fragments that render this tag.</returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect)
    {

        foreach(ISqlFragment sqlFragment in SqlExpression.ToSqlFragments(dialect).Flatten(dialect))
        {
            yield return sqlFragment;
        }
        if(AliasTag is not null)
        {
            yield return new SqlFragmentText(" AS ");
            yield return AliasTag;
        }
    }

    /// <summary>
    /// Gets the SQL parameters contained by this fragment.
    /// </summary>
    /// <returns>The SQL parameters referenced by the projected expression.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        SqlExpression.GetSqlFragmentParameters(dialect);

    /// <summary>
    /// Renders the selected expression and optional alias using the supplied SQL dialect.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render identifiers.</param>
    /// <returns>The rendered SELECT-list fragment.</returns>
    public string ToSql(ISqlDialects dialect) =>
        Flatten(dialect).ToSql(dialect);

    /// <summary>
    /// Returns the dialect-neutral diagnostic representation of this select item.
    /// </summary>
    public override string ToString() =>
        ToSql(NeutralDialect.Instance);

    /// <summary>
    /// Determines whether this select item projects the same expression with the same alias as another select item.
    /// </summary>
    /// <param name="other">The other select item to compare.</param>
    /// <returns><c>true</c> when both the expression and alias are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(SelectTag? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        return SqlExpression.Equals(other.SqlExpression) && EqualityComparer<AliasTag?>.Default.Equals(AliasTag, other.AliasTag);
    }

    /// <summary>
    /// Determines whether the specified object represents an equivalent select item.
    /// </summary>
    public override bool Equals(object? obj) =>
        Equals(obj as SelectTag);

    /// <summary>
    /// Returns a hash code based on the projected expression and alias.
    /// </summary>
    public override int GetHashCode() =>
        HashCode.Combine(SqlExpression, AliasTag);

    /// <summary>
    /// Determines whether two select items project equivalent expressions with equivalent aliases.
    /// </summary>
    public static bool operator ==(SelectTag? left, SelectTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two select items differ by expression or alias.
    /// </summary>
    public static bool operator !=(SelectTag? left, SelectTag? right) =>
        (left == right) == false;

    /// <summary>
    /// Indicates whether this select item matches the supplied <c>GROUP BY</c> clause.
    /// </summary>
    /// <param name="groupByBase">
    /// The <c>GROUP BY</c> clause to compare against this select item.
    /// </param>
    /// <returns>
    /// <c>true</c> if this select item matches the supplied <c>GROUP BY</c> clause; otherwise, <c>false</c>.
    /// </returns>
    [Obsolete("No longer needed, useful or recommended.")]
    public bool MatchesGroupBy(GroupBy groupByBase) =>
        groupByBase.SqlExpression.Equals(SqlExpression);

    /// <summary>
    /// Creates a new <see cref="SelectTag"/> instance with the same column as the current instance but without any alias.
    /// </summary>
    public virtual SelectTag WithNoAlias() =>
        new(SqlExpression);
}
