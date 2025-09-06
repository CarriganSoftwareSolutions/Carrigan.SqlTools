
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlToolsTests.TestEntities;

[Table("Left")]
public class JoinLeftTable
{
    [Key]
    public int Id { get; set; }

    public int RightId { get; set; }

    public string Col1 { get; set; } = string.Empty;
    public string Col2 { get; set; } = string.Empty;
    public string Col3 { get; set; } = string.Empty;
}
