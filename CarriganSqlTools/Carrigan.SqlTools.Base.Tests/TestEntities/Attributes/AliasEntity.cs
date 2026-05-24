using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
internal class AliasEntity
{
    public int Id { get; set; }
    [Alias("AnAlias")]
    public string? TestColumn { get; set; }
    public string? NoAlias { get; set; }
}
