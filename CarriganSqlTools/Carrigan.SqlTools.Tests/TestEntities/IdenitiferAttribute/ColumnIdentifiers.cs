using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
public class ColumnIdentifiers
{
    [Key]
    public int Id { get; set; }
    public int Property { get; set; }
    [Column("Column")]
    public int ColumnName { get; set; }
    [Identifier("Identifier")]
    public int IdentifierName { get; set; }
    [Identifier("IdentifierOverride")]
    [Column("Column")]
    public int IdentifierOverrideName { get; set; }
}
