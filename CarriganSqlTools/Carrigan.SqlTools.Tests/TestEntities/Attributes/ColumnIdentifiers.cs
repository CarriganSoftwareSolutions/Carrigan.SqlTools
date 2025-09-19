using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Attributes;
public class ColumnIdentifiers
{
    [Key]
    [Parameter("IdParameter")]
    public int Id { get; set; }
    [Parameter("PropertyParameter")]
    public int Property { get; set; }
    [Parameter("ColumnParameter")]
    [Column("Column")]
    public int ColumnName { get; set; }
    [Parameter("IdentifierParameter")]
    [Identifier("Identifier")]
    public int IdentifierName { get; set; }
    [Parameter("IdentifierOverrideParameter")]
    [Identifier("IdentifierOverride")]
    [Column("Column")]
    public int IdentifierOverrideName { get; set; }
}
