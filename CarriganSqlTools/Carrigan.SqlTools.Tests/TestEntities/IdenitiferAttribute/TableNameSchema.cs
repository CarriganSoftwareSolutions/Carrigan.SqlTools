using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Table("TableNameSchemaTable", Schema="Table")]
internal class TableNameSchema
{
    Guid Id { get; set; }
    string? Text { get; set; }
}
