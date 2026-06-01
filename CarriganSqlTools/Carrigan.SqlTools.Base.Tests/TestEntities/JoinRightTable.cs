using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Carrigan.SqlTools.Base.Tests.TestEntities;

[Identifier("Right")]
internal class JoinRightTable
{
    [Key]
    public int Id { get; set; }
    public int LastId { get; set; }
    public string Col1 { get; set; } = string.Empty;
    public string Col2 { get; set; } = string.Empty;
    public string Col3 { get; set; } = string.Empty;
}
