using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities;


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

    public Address Address { get; set; }
}