using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities;

[Table("Phone", Schema="schema")]
internal class PhoneModel
{
    [Key]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    [Column("Phone")]
    public string? PhoneNumber { get; set; }
}
