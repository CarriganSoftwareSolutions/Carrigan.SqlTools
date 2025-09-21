using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Carrigan.SqlTools.Tests.TestEntities;

internal class Customer
{
    [PrimaryKey] //note: PrimaryKey take precedence over key for the Sql Generator
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    [Key]
    public string Phone { get; set; } = "";
}
