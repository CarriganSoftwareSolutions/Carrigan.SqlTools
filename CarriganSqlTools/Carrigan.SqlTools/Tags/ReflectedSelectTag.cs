using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Internal neutral select tag used by reflection metadata and attributes when no dialect-specific
/// concrete select tag is required.
/// </summary>
internal sealed class ReflectedSelectTag : SelectTagBase
{
    private readonly ReflectedSelectTag? WithNoAliasProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReflectedSelectTag"/> class.
    /// </summary>
    /// <param name="columnTag">
    /// The <see cref="ColumnTag"/> representing the column metadata for this select tag.
    /// </param>
    /// <param name="aliasTag">
    /// The <see cref="AliasTag"/> representing the alias metadata for this select tag.
    /// </param>
    internal ReflectedSelectTag(ColumnTag columnTag, AliasTag? aliasTag = null)
        : this(new ColumnTagExpression(columnTag), aliasTag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReflectedSelectTag"/> class from a resolved SQL expression.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression represented by this select tag.</param>
    /// <param name="aliasTag">The optional alias applied to the selected expression.</param>
    internal ReflectedSelectTag(SqlExpression sqlExpression, AliasTag? aliasTag = null)
        : base(sqlExpression, aliasTag)
    {
        if (aliasTag is not null)
            WithNoAliasProperty = new(sqlExpression);
        else
            WithNoAliasProperty = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WithNoAlias"/> class.
    /// </summary>
    public override SelectTagBase WithNoAlias() =>
        WithNoAliasProperty is null ? this : WithNoAliasProperty;
}
