using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Base.Tests.TestEntities;


//Note: Table's name attribute of "Test" does not affect the Procedure name when the table is used a procedure model.
[Table("Test")]
public partial class EntityWithTableAttribute
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public EntityWithTableAttribute() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    [NotMapped]
    public string? Where { get; set; }

    public DateTime DateOf { get; set; }

    [NotMapped]
    public bool HideTimeFlag { get; set; }

    public string? When { get; set; }

    /// <summary>
    /// This property is not mapped to the database because it is a complex type. 
    /// Complex types are not directly mapped to database columns, and therefore, this property will not be included in the database schema.
    /// </summary>
    public Address Address { get; set; }
}