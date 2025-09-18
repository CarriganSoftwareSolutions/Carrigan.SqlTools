using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Identifier("IdentifierNameSchemaTable", "Identifier")]
public class IdentifierNameSchema
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
}
