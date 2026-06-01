using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Aliases;

internal class AliasNameEmpty
{
    [Alias("")]
    public int ExceptionColumn { get; set; }
}
