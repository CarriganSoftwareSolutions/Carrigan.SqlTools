using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Identifier("IdentifierNameSchemaTable", "Identifier")]
internal class IdentifierNameSchema
{
    internal Guid Id { get; set; }
    internal string? Text { get; set; }
}
