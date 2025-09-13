
using Carrigan.Core.Attributes;
using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities;

//ignore spelling: myschema

[Table("EntityWithSchema", Schema = "myschema")]
[Procedure("EntityWithSchema", Schema: "myschema")]
public class EntityWithSchema
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public EntityWithSchema() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Key]
    public int Id { get; set; }
    public string Description { get; set; }
}
