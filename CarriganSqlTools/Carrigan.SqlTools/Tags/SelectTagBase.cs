using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a SELECT projection tag for a single column, consisting of a fully qualified
/// column identifier and an optional alias.
/// </summary>
public abstract class SelectTagBase : StringWrapper, ISqlFragment
{
    /// <summary>
    /// The fully qualified column identifier for this select item.
    /// </summary>
    internal readonly ColumnTag ColumnTag;

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
        : this(CreateColumnTag(propertyName), AliasTag.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTagBase"/> class.
    /// </summary>
    /// <param name="columnTag">The column identifier to select.</param>
    /// <param name="aliasTag">The optional alias to apply to the selected column.</param>
    internal SelectTagBase(ColumnTag columnTag, AliasTag? aliasTag = null)
        : base(aliasTag.IsNullOrWhiteSpace() ? columnTag : $"{columnTag} AS {aliasTag}")
    {
        ColumnTag = columnTag;
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

        return new(new ColumnName(propertyName));
    }

    /// <summary>
    /// Gets the expected result set column name for this projection, choosing the alias
    /// when present or the underlying column name when no alias exists.
    /// </summary>
    internal ResultColumnName ResultColumnName =>
        new(AliasTag?.ToString() ?? ColumnTag.ColumnName);

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
    /// Renders the selected column and optional alias using the supplied SQL dialect.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render identifiers.</param>
    /// <returns>The rendered SELECT-list fragment.</returns>
    public string ToSql(ISqlDialects dialect) =>
        AliasTag is null ? ColumnTag.ToSql(dialect) : $"{ColumnTag.ToSql(dialect)} AS {AliasTag.ToSql(dialect)}";

    /// <summary>
    /// Creates an equivalent select tag without an alias.
    /// </summary>
    /// <returns>A copy of this tag without an alias.</returns>
    public abstract SelectTagBase WithNoAlias();
}
