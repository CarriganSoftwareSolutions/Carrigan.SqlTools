using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Identifier("IdentifierNameTable")]
internal class IdentifierName
{
    internal Guid Id { get; set; }
    internal string? Text { get; set; }
}
