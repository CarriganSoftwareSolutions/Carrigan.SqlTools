using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Identifier("IdentifierNameOverrideSchemaTable", "Identifier")]
[Table("WtfMate")]
public class IdentifierNameOverrideSchema
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
}
