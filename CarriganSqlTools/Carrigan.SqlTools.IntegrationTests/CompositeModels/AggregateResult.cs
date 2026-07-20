using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.IntegrationTests.CompositeModels;

public class AggregateResult
{
    public char? Gender { get; set; }
    public string? Title { get; set; }
    public decimal? Average { get; set; }
    public int? Max { get; set; }
    public int? Min { get; set; }
    public int? Count { get; set; }
    public int? Sum { get; set; }
}
