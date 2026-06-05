namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Internal neutral select tag used by reflection metadata and attributes when no dialect-specific
/// concrete select tag is required.
/// </summary>
internal sealed class ReflectedSelectTag : SelectTagBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReflectedSelectTag"/> class.
    /// </summary>
    /// <param name="columnTag">
    /// The <see cref="ColumnTag"/> representing the column metadata for this select tag.
    /// </param>
    /// <param name="aliasTag">
    /// The <see cref="AliasTag"/> representing the alias metadata for this select tag.
    /// </param>
    internal ReflectedSelectTag(ColumnTag columnTag, AliasTag? aliasTag = null) : base(columnTag, aliasTag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WithNoAlias"/> class.
    /// </summary>
    public override SelectTagBase WithNoAlias() =>
        new ReflectedSelectTag(ColumnTag);
}
