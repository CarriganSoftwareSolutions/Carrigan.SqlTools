using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities;

[Identifier("Phone", "schema")]
internal class PhoneModel
{
    [PrimaryKey]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    [Identifier("Phone")]
    public string? PhoneNumber { get; set; }
}
