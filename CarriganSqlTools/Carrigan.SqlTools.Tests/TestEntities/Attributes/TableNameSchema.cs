using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Attributes;
[Table("TableNameSchemaTable", Schema="Table")]
public class TableNameSchema
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
}
