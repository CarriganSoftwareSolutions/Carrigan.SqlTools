
using System.ComponentModel.DataAnnotations;

namespace Carrigan.SqlTools.Tests.TestEntities;
internal class Order
{
    [Key] //Required attribute for certain SQL Generations
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
}
