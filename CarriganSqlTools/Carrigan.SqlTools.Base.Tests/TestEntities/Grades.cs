namespace Carrigan.SqlTools.Base.Tests.TestEntities;

public class Grades
{
    public Guid StudentId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public int AcademicYear { get; set; }
    public int SemesterNumber { get; set; }
    public decimal GradePoint { get; set; }
    public int CreditHours { get; set; }
}
