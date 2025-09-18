using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Table("TableNameSchemaTable", Schema="Table")]
internal class TableNameSchema
{
    internal Guid Id { get; set; }
    internal string? Text { get; set; }
}
