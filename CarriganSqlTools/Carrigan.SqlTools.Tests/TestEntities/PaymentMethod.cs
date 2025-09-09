using System.ComponentModel.DataAnnotations;

namespace Carrigan.SqlTools.Tests.TestEntities;
//IGNORE SPELLING: Cvv

internal class PaymentMethod
{
    [Key] //Required attribute for certain SQL Generations
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string CardNumber { get; set; } = "";
    public int Cvv { get; set; } = 0;
    public int ExpirationYear { get; set; } = 0;
    public int ExpirationMonth { get; set; } = 0;
    public string ZipCode { get; set; } = "";
}
