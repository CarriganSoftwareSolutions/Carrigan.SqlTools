using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Identifier("IdentifierNameOverrideSchemaTable", "Identifier")]
[Table("WtfMate")]
internal class IdentifierNameOverrideSchema
{
    internal Guid Id { get; set; }
    internal string? Text { get; set; }
}
