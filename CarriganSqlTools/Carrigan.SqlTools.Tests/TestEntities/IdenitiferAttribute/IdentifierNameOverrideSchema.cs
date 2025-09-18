using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Identifier("IdentifierNameOverrideSchemaTable", "Identifier")]
[Table("WtfMate")]
internal class IdentifierNameOverrideSchema
{
    Guid Id { get; set; }
    string? Text { get; set; }
}
