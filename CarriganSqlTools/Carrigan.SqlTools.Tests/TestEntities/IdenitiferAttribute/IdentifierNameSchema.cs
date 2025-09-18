using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Identifier("IdentifierNameSchemaTable", "Identifier")]
internal class IdentifierNameSchema
{
    Guid Id { get; set; }
    string? Text { get; set; }
}
