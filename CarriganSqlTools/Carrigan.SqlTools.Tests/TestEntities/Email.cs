using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities;

[Identifier("Email", "schema")]
public class EmailModel
{
    [PrimaryKey]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    [Identifier("Email")]
    public string? EmailAddress { get; set; }
}