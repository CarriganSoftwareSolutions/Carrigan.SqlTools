using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Identifier("IdentifierNameTable")]
internal class IdentifierName
{
    Guid Id { get; set; }
    string? Text { get; set; }
}
