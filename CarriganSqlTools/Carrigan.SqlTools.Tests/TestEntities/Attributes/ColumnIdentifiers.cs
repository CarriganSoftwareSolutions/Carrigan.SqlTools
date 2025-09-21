using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Attributes;
public class ColumnIdentifiers
{
    [PrimaryKey] //note: PrimaryKey take precedence over key for the Sql Generator
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
    [Key]
    [Parameter("IdentifierOverrideParameter")]
    [Identifier("IdentifierOverride")]
    [Column("Column")]
    public int IdentifierOverrideName { get; set; }
}
