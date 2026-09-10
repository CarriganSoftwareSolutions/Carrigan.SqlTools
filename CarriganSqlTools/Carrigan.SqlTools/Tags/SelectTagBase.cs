using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
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
public abstract class SelectTagBase :
    IEquatable<SelectTagBase>,
    IEqualityOperators<SelectTagBase, SelectTagBase, bool>,
    ISqlFragment
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
    /// Initializes a new instance of the <see cref="SelectTagBase"/> class from a property name.
    /// </summary>
    /// <param name="propertyName">The model property/column name to select.</param>
    /// <param name="aliasName">The optional alias to apply to the selected column.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    protected SelectTagBase(PropertyName propertyName, AliasName? aliasName = null)
        : this(new ColumnTagExpression(CreateColumnTag(propertyName)), AliasTag.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTagBase"/> class.
    /// </summary>
    /// <param name="columnTag">The column identifier to select.</param>
    /// <param name="aliasTag">The optional alias to apply to the selected column.</param>
    internal SelectTagBase(ColumnTag columnTag, AliasTag? aliasTag = null)
        : this(new ColumnTagExpression(columnTag), aliasTag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTagBase"/> class.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to select.</param>
    /// <param name="aliasTag">The optional alias to apply to the selected expression.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlExpression"/> is <c>null</c>.
    /// </exception>
    protected SelectTagBase(SqlExpression sqlExpression, AliasTag? aliasTag = null)
    {
        ArgumentNullException.ThrowIfNull(sqlExpression, nameof(sqlExpression));

        SqlExpression = sqlExpression;
        AliasTag = aliasTag;
    }


    /// <summary>
    /// Creates a column tag from a model property name when no reflected table context is available.
    /// </summary>
    /// <param name="propertyName">The model property name to use as the SQL column name.</param>
    /// <returns>A column tag containing the column name derived from <paramref name="propertyName"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="propertyName"/> is <see langword="null"/>.
    /// </exception>
    private static ColumnTag CreateColumnTag(PropertyName propertyName)
    {
        ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

        return new(new ColumnName(propertyName.ToString()));
    }

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
        SqlExpression.DescendantLeafTables;

    /// <summary>
    /// Gets the expected result set column name for this projection, choosing the alias
    /// when present, the underlying column name for simple columns, or the expression text otherwise.
    /// </summary>
    internal ResultColumnName ResultColumnName =>
        new(AliasTag?.ToString() ?? GetSimpleColumnTag(SqlExpression)?.ColumnName.ToString() ?? SqlExpression.ToString());

    /// <summary>
    /// Indicates whether this select item is valid in an aggregate SELECT list for the supplied <c>GROUP BY</c> clause.
    /// </summary>
    /// <returns>The aggregate status of the underlying expression or column.</returns>
    public bool IsAggregate() =>
        SqlExpression.IsAggregate();

    /// <summary>
    /// Flattens this fragment into the sequence of fragments used to render SQL text.
    /// </summary>
    /// <returns>A flattened sequence of SQL fragments that render this tag.</returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect)
    {

        foreach(ISqlFragment sqlFragment in SqlExpression.ToSqlFragments(dialect))
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
    /// <returns>An empty sequence because SELECT projection fragments do not contain SQL parameters.</returns>
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
        AliasTag.IsNotNullOrWhiteSpace() ? $"{SqlExpression} AS {AliasTag}" : SqlExpression.ToString() ?? string.Empty;

    /// <summary>
    /// Determines whether this select item projects the same expression with the same alias as another select item.
    /// </summary>
    /// <param name="other">The other select item to compare.</param>
    /// <returns><c>true</c> when both the expression and alias are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(SelectTagBase? other)
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
        Equals(obj as SelectTagBase);

    /// <summary>
    /// Returns a hash code based on the projected expression and alias.
    /// </summary>
    public override int GetHashCode() =>
        HashCode.Combine(SqlExpression, AliasTag);

    /// <summary>
    /// Determines whether two select items project equivalent expressions with equivalent aliases.
    /// </summary>
    public static bool operator ==(SelectTagBase? left, SelectTagBase? right)
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
    public static bool operator !=(SelectTagBase? left, SelectTagBase? right) =>
        (left == right) == false;

    /// <summary>
    /// Creates an equivalent select tag without an alias.
    /// </summary>
    /// <returns>A copy of this tag without an alias.</returns>
    public abstract SelectTagBase WithNoAlias();

    /// <summary>
    /// Indicates whether this select item matches the supplied <c>GROUP BY</c> clause.
    /// </summary>
    /// <param name="groupByBase">
    /// The <c>GROUP BY</c> clause to compare against this select item.
    /// </param>
    /// <returns>
    /// <c>true</c> if this select item matches the supplied <c>GROUP BY</c> clause; otherwise, <c>false</c>.
    /// </returns>
    public bool MatchesGroupBy(GroupByBase groupByBase)
    {
        ColumnTag? columnTag = SqlExpression switch
        {
            IColumnBase column => column.ColumnInfo.ColumnTag,
            ColumnTagExpression columnTagExpression => columnTagExpression.ColumnTag,
            _ => null
        };

        return columnTag is not null && columnTag == groupByBase.ColumnInfo.ColumnTag;
    }
}
