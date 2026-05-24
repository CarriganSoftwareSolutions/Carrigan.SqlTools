using Carrigan.Core.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals;
public class NoKeyVersionPropertyEntity
{
    [Encrypted]
    public string? Encrypted { get; set; }
}
