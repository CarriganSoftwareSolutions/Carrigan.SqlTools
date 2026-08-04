namespace Carrigan.SqlTools.IntegrationTests.CompositeModels;

public sealed class GradeResults
{
    public Guid StudentId { get; set; }
    public int AcademicYear { get; set; }
    public int SemesterNumber { get; set; }
    public decimal AverageGPA { get; set; }
    public decimal MinGPA { get; set; }
    public decimal MaxGPA { get; set; }
    public int Count { get; set; }
}
