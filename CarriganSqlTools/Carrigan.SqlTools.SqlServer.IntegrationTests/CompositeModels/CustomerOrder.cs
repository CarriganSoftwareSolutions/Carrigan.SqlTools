using System;
using System.Collections.Generic;
using System.Text;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Models;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.CompositeModels;

internal class CustomerOrder
{
    [SelectTag<Customer>(nameof(Customer.Id), nameof(CustomerId))]
    public int CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public char Gender { get; set; }
    public int OrderId { get; set; }
    public int AddressId { get; set; }
    public DateOnly Date { get; set; }
    public decimal SalesTaxPercent { get; set; }
}
