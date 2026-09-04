using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;

public class ParameterAttributeEntity
{
    [Parameter("IdParameter")]
    public int Id { get; set; }

    [Parameter("DescriptionParameter")]
    public string Description { get; set; } = string.Empty;

    [Parameter("EnabledParameter")]
    public bool Enabled { get; set; }

    public int UnattributedNumeric { get; set; }

    public bool UnattributedBoolean { get; set; }
}
