
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlToolsTests.TestEntities;

[Table("Right")]
internal class JoinRightTable
{
    [Key]
    public int Id { get; set; }
    public int LastId { get; set; }
    public string Col1 { get; set; } = string.Empty;
    public string Col2 { get; set; } = string.Empty;
    public string Col3 { get; set; } = string.Empty;
}
