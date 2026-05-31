using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Internal neutral select tag used by reflection metadata and attributes when no dialect-specific
/// concrete select tag is required.
/// </summary>
internal sealed class ReflectedSelectTag : SelectTagBase
{
    internal ReflectedSelectTag(ColumnTag columnTag, AliasTag? aliasTag = null) : base(columnTag, aliasTag)
    {
    }

    public override SelectTagBase WithNoAlias() =>
        new ReflectedSelectTag(ColumnTag);
}
