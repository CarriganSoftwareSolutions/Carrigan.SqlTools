using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities;

[Table("Ck")]
internal class CompositePrimaryKeyTable
{
    [PrimaryKey] //note: PrimaryKey take precedence over key for the Sql Generator
    public int Id1 { get; set; }
    [PrimaryKey] //note: PrimaryKey take precedence over key for the Sql Generator
    [Key]
    public int Id2 { get; set; }

    public int NotKey1 { get; set; }
    public int NotKey2 { get; set; }
    public int NotKey3 { get; set; }
}
