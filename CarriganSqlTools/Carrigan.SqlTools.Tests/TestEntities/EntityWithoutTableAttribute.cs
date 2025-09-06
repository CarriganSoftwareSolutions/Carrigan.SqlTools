
using System.ComponentModel.DataAnnotations;

namespace SqlToolsTests.TestEntities;

// Additional entity classes for testing
public class EntityWithoutTableAttribute
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public EntityWithoutTableAttribute() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Key]
    public int Id { get; set; }
    public string Description { get; set; }
}
