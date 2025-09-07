using System.ComponentModel.DataAnnotations;

namespace Carrigan.SqlTools.Tests.TestEntities;

internal class Customer
{
    [Key] //Required attribute for certain SQL Generations
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}
