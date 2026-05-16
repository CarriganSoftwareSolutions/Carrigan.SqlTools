using Carrigan.SqlTools.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.IntegrationTests.Models;

internal class BookStatsSelects
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float AverageReview { get; set; }
    public int YearPublished { get; set; }
}
