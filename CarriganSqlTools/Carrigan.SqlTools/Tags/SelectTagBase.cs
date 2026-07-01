using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a SELECT projection tag for a single SQL expression, consisting of the expression
/// and an optional alias.
/// </summary>
public abstract class SelectTagBase : StringWrapper, ISqlFragment
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
        : base(CreateBaseValue(sqlExpression, aliasTag))
    {
        ArgumentNullException.ThrowIfNull(sqlExpression, nameof(sqlExpression));

        SqlExpression = sqlExpression;
        AliasTag = aliasTag;
    }


    /// <summary>
    /// Creates the string-wrapper value for an expression select item.
    /// </summary>
    private static string CreateBaseValue(SqlExpression sqlExpression, AliasTag? aliasTag)
    {
        ArgumentNullException.ThrowIfNull(sqlExpression, nameof(sqlExpression));

        if (aliasTag.IsNotNullOrWhiteSpace())
            return $"{sqlExpression} AS {aliasTag}";
        else
            return sqlExpression.ToString();
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

        return new(new ColumnName(propertyName));
    }

    /// <summary>
    /// Gets the simple column tag represented by a column-shaped expression, when applicable.
    /// </summary>
    private static ColumnTag? GetSimpleColumnTag(SqlExpression sqlExpression) =>
        sqlExpression switch
        {
            ColumnBase column => column.ColumnInfo.ColumnTag,
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
    /// <param name="groupBys">The optional <c>GROUP BY</c> clause to check.</param>
    /// <returns>The aggregate status of the underlying expression or column.</returns>
    public bool IsAggregate(GroupBysBase? groupBys) =>
        SqlExpression.IsAggregate(groupBys);

    /// <summary>
    /// Flattens this fragment into the sequence of fragments used to render SQL text.
    /// </summary>
    /// <returns>A flattened sequence of SQL fragments that render this tag.</returns>
    public IEnumerable<ISqlFragment> Flatten()
    {
        yield return this;
    }

    /// <summary>
    /// Gets the SQL parameters contained by this fragment.
    /// </summary>
    /// <returns>An empty sequence because SELECT projection fragments do not contain SQL parameters.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters() =>
        [];

    /// <summary>
    /// Renders the selected expression and optional alias using the supplied SQL dialect.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render identifiers.</param>
    /// <returns>The rendered SELECT-list fragment.</returns>
    public string ToSql(ISqlDialects dialect)
    {
        string selectSql = SqlExpression.ToSqlFragments(dialect).ToSql(dialect);

        return AliasTag is null ? selectSql : $"{selectSql} AS {AliasTag.ToSql(dialect)}";
    }

    /// <summary>
    /// Creates an equivalent select tag without an alias.
    /// </summary>
    /// <returns>A copy of this tag without an alias.</returns>
    public abstract SelectTagBase WithNoAlias();
}
