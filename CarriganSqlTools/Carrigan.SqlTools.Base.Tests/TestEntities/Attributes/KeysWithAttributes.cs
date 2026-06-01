using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
internal class PrimaryKeysWithAttributes
{
    [PrimaryKey]
    public int Id1 { get; set; }
    [PrimaryKey]
    [Column("IdTwo")]
    public int Id2 { get; set; }
    [PrimaryKey]
    [Identifier("IdThree")]
    [Column("Id3")]
    public int Id3 { get; set; }
}
