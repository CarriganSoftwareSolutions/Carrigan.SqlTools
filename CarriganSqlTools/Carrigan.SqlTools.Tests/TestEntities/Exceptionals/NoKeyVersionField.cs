using Carrigan.Core.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals;
public class NoKeyVersionField
{
    [Encrypted]
    public string? Encrypted { get; set; }
}
