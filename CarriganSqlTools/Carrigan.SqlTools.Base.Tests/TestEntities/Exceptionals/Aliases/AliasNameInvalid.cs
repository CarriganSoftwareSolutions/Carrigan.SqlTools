using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Aliases;
internal class AliasNameInvalid
{
    [Alias("123")]
    public int ExceptionColumn { get; set; }
}
