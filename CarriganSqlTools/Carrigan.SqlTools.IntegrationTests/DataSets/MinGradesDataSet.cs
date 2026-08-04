using Carrigan.SqlTools.IntegrationTests.CompositeModels;

namespace Carrigan.SqlTools.IntegrationTests.DataSets;

public static class MinGradesDataSet
{
    public static void Validate(IEnumerable<GradeResults> actualResults) =>
        GradeResultsDataSetValidation.Validate(actualResults, Data);

    public static readonly IEnumerable<GradeResults> Data =
    [
        new()
        {
            StudentId = new("11ff6f85-8a27-4a30-b48a-2c2181c884c4"),
            AcademicYear = 2010,
            SemesterNumber = 2,
            AverageGPA = 1.300000m,
            MinGPA = 1.30m,
            MaxGPA = 1.30m,
            Count = 1,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            AcademicYear = 2002,
            SemesterNumber = 1,
            AverageGPA = 1.300000m,
            MinGPA = 1.30m,
            MaxGPA = 1.30m,
            Count = 1,
        },
        new()
        {
            StudentId = new("727720e6-3577-4bd6-a419-7c85684d0740"),
            AcademicYear = 2004,
            SemesterNumber = 3,
            AverageGPA = 0.700000m,
            MinGPA = 0.70m,
            MaxGPA = 0.70m,
            Count = 1,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            AcademicYear = 2001,
            SemesterNumber = 2,
            AverageGPA = 2.500000m,
            MinGPA = 1.30m,
            MaxGPA = 3.70m,
            Count = 2,
        },
        new()
        {
            StudentId = new("b657493e-e7cf-4b67-8535-3a5f19ac406d"),
            AcademicYear = 2007,
            SemesterNumber = 3,
            AverageGPA = 1.300000m,
            MinGPA = 1.30m,
            MaxGPA = 1.30m,
            Count = 1,
        },
        new()
        {
            StudentId = new("b657493e-e7cf-4b67-8535-3a5f19ac406d"),
            AcademicYear = 2008,
            SemesterNumber = 1,
            AverageGPA = 1.000000m,
            MinGPA = 1.00m,
            MaxGPA = 1.00m,
            Count = 1,
        },
        new()
        {
            StudentId = new("b657493e-e7cf-4b67-8535-3a5f19ac406d"),
            AcademicYear = 2008,
            SemesterNumber = 2,
            AverageGPA = 0.700000m,
            MinGPA = 0.70m,
            MaxGPA = 0.70m,
            Count = 1,
        },
        new()
        {
            StudentId = new("d7a2bf3b-98a9-49dc-aae9-50907f82117e"),
            AcademicYear = 2005,
            SemesterNumber = 3,
            AverageGPA = 1.300000m,
            MinGPA = 1.30m,
            MaxGPA = 1.30m,
            Count = 1,
        },
        new()
        {
            StudentId = new("e7c667bd-641c-46f5-a7e5-415597197313"),
            AcademicYear = 2009,
            SemesterNumber = 2,
            AverageGPA = 1.300000m,
            MinGPA = 1.30m,
            MaxGPA = 1.30m,
            Count = 1,
        },
        new()
        {
            StudentId = new("e8302c29-9464-47ac-bdd5-944b4753e90e"),
            AcademicYear = 2004,
            SemesterNumber = 1,
            AverageGPA = 1.300000m,
            MinGPA = 1.30m,
            MaxGPA = 1.30m,
            Count = 1,
        },
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            AcademicYear = 2004,
            SemesterNumber = 2,
            AverageGPA = 1.000000m,
            MinGPA = 1.00m,
            MaxGPA = 1.00m,
            Count = 1,
        },
    ];
}
