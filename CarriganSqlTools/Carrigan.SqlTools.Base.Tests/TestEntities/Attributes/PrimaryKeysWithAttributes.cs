using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
internal class KeysWithAttributes
{
    [Key]
    public int Id1 { get; set; }
    [Key]
    [Column("IdTwo")]
    public int Id2 { get; set; }
    [Key]
    [Identifier("IdThree")]
    [Column("Id3")]
    public int Id3 { get; set; }
}
