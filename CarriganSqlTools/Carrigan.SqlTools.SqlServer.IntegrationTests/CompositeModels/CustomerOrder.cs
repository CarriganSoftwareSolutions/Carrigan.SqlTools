using System;
using System.Collections.Generic;
using System.Text;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.CompositeModels;

internal class CustomerOrder
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public char Gender { get; set; }
}
