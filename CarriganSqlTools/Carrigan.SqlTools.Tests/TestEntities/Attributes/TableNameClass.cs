using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Attributes;
[Table("TableNameTable")]
public class TableNameClass
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
}
