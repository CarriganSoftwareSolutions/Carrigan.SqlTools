
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Models;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.CompositeModels;

internal class CustomerOrder
{
    [SelectTag<Customer>(nameof(Customer.Id), nameof(CustomerId))]
    public int? CustomerId { get; set; }
    [SelectTag<Customer>(nameof(Customer.FirstName))]
    public string? FirstName { get; set; } = string.Empty;
    [SelectTag<Customer>(nameof(Customer.LastName))]
    public string? LastName { get; set; } = string.Empty;
    [SelectTag<Customer>(nameof(Customer.Age))]
    public int? Age { get; set; }
    [SelectTag<Customer>(nameof(Customer.Gender))]
    public char? Gender { get; set; }

    [SelectTag<Order>(nameof(Order.Id), nameof(OrderId))]
    public int? OrderId { get; set; }

    [SelectTag<Order>(nameof(Order.CustomerId), nameof(OrderCustomerId))]
    public int? OrderCustomerId { get; set; }
    [SelectTag<Order>(nameof(Order.AddressId))]
    public int? AddressId { get; set; }
    [SelectTag<Order>(nameof(Order.Date))]
    public DateOnly? Date { get; set; }
    [SelectTag<Order>(nameof(Order.SalesTaxPercent))]
    public decimal? SalesTaxPercent { get; set; }
}
