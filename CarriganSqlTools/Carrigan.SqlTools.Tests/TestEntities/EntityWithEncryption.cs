using Carrigan.Core.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities;

[Table("Test")]
[Procedure("TestProcedure")]
internal class EntityWithEncryption
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public EntityWithEncryption() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [Key]
    public int Id { get; set; }
    public string NotSensitiveData { get; set; }
    [Encrypted]
    public string? SensitiveData { get; set; }
    [KeyVersion]
    public int KeyVersion { get; set; }
}
