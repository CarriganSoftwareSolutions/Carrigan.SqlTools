using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.Attributes;
[Identifier("IdentifierNameSchemaTable", "Identifier")]
public class IdentifierNameSchema
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
}
