using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Aliases;
internal class AliasNameNull
{
    [Alias(null!)]
    public int ExceptionColumn { get; set; }
}
