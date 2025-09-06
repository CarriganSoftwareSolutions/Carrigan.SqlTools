using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities;

[Table("Ck")]
internal class CompositeKeyTable
{
    [Key]
    public int Id1 { get; set; }
    [Key]
    public int Id2 { get; set; }

    public int NotKey1 { get; set; }
    public int NotKey2 { get; set; }
    public int NotKey3 { get; set; }
}
