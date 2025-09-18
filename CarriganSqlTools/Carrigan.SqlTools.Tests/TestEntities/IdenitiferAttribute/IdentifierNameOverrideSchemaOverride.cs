using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Identifier("IdentifierNameOverrideSchemaOverrideTable", "Identifier")]
[Table("WtfMate", Schema = "Table")]
public class IdentifierNameOverrideSchemaOverride
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
}
