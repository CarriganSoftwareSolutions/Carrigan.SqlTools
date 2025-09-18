using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Identifier("IdentifierNameOverrideSchemaOverrideTable", "Identifier")]
[Table("WtfMate", Schema = "Table")]
internal class IdentifierNameOverrideSchemaOverride
{
    internal Guid Id { get; set; }
    internal string? Text { get; set; }
}
