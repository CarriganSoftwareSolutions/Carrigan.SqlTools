using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.Attributes;
[Identifier("IdentifierNameTable")]
public class IdentifierName
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
}
