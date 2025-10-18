using Carrigan.Core.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals;
public class NoKeyVersionPropertyEntity
{
    [Encrypted]
    public string? Encrypted { get; set; }
}
