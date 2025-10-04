using Carrigan.Core.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals;
public class MultiKeyVersions
{
    [KeyVersion]
    public int KeyVersion1 { get; set; } = 1;
    [KeyVersion]
    public int KeyVersion2 { get; set; } = 2;
    [Encrypted]
    public string? Encrypted { get; set; }
}
