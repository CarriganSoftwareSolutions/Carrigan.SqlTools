using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities;

//Note: Identifier "Left" should override the Table's name attribute
[Identifier("Left")]
[Table("LeftTable")]
public class JoinLeftTable
{
    [Key]
    public int Id { get; set; }

    public int RightId { get; set; }

    public string Col1 { get; set; } = string.Empty;
    public string Col2 { get; set; } = string.Empty;
    public string Col3 { get; set; } = string.Empty;
}
