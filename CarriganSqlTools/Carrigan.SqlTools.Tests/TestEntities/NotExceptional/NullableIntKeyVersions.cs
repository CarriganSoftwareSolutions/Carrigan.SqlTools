using Carrigan.Core.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.NotExceptional;
public class NullableIntKeyVersions
{
    [KeyVersion]
    public int? KeyVersion { get; set; }
    [Encrypted]
    public string? Encrypted { get; set; }
}
