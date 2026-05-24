using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
[Identifier("IdentifierNameOverrideTable")]
[Table("WtfMate")]
public class IdentifierNameOverride
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
}

