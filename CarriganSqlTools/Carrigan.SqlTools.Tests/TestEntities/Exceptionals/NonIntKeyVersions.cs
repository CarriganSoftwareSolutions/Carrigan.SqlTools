using Carrigan.Core.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals;
public class NonIntKeyVersions
{
    [KeyVersion]
    public bool KeyVersion { get; set; } = true;
    [Encrypted]
    public string? Encrypted { get; set; }
}
