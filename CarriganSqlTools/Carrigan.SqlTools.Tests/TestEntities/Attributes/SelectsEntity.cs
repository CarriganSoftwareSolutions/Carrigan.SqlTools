using Carrigan.SqlTools.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tests.TestEntities.Attributes;
[Identifier("RedHerring")] //just to make sure it doesn't get confused by this.
internal class SelectsEntity
{
    [PrimaryKey]
    public int Id { get; set; } //Note: Correct Select Name is "Id"
    public int Property { get; set; } //Note: Correct Select Name is "Property"

    [Column("Column")]
    public int ColumnName { get; set; } //Note: Correct Select Name is "Column"

    [Identifier("Identifier")]
    public int IdentifierName { get; set; } //Note: Correct Select Name is "Identifier"

    [Identifier("IdentifierOverride")]
    [Column("Column")]
    public int IdentifierOverrideName { get; set; } //Note: Correct Select Name is "IdentifierOverride"

    [Alias("Alias")]
    public int AliasName { get; set; } //Note: Correct Select Name is "Alias"

    [Alias("AliasOverride")]
    [Identifier("IdentifierOverride")]
    [Column("Column")]
    public int AliasOverrideName { get; set; } //Note: Correct Select Name is "AliasOverride"
}
