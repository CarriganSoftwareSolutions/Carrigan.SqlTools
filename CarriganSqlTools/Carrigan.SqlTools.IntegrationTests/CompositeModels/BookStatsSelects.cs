namespace Carrigan.SqlTools.IntegrationTests.CompositeModels;

public class BookStatsSelects
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal AverageReview { get; set; }
    public int YearPublished { get; set; }
}
