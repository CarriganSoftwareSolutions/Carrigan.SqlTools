using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.SqlTypes;

public sealed class SqlTypeMismatchImageAttributeEntity
{
    public int Id { get; set; }

    [SqlImage]
    public string? NotByteArray { get; set; }
}
