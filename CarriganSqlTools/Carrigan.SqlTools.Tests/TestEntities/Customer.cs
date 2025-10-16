using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities;

internal class Customer
{
    [PrimaryKey] //note: PrimaryKey take precedence over key for the Sql Generator
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
}
