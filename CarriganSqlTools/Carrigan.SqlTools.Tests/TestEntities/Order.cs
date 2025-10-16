using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities;
internal class Order
{
    [PrimaryKey] //Required attribute for certain SQL Generations
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int PaymentMethodId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
}
