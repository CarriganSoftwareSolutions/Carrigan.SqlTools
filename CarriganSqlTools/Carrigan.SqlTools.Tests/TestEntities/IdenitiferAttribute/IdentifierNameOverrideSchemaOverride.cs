using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Identifier("IdentifierNameOverrideSchemaOverrideTable", "Identifier")]
[Table("WtfMate", Schema = "Table")]
internal class IdentifierNameOverrideSchemaOverride
{
    Guid Id { get; set; }
    string? Text { get; set; }
}
