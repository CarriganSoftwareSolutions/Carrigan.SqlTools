using Carrigan.SqlTools.IntegrationTests.CompositeModels;

namespace Carrigan.SqlTools.IntegrationTests.DataSets;

public static class AverageGradesDataSet
{
    public static void Validate(IEnumerable<GradeResults> actualResults) =>
        GradeResultsDataSetValidation.Validate(actualResults, Data);

    public static readonly IEnumerable<GradeResults> Data =
    [
        new()
        {
            StudentId = new("5036baa8-4096-4f4c-9816-f3a44547b7be"),
            AcademicYear = 2008,
            SemesterNumber = 1,
            AverageGPA = 3.700000m,
            MinGPA = 3.70m,
            MaxGPA = 3.70m,
            Count = 1,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            AcademicYear = 2002,
            SemesterNumber = 3,
            AverageGPA = 3.700000m,
            MinGPA = 3.70m,
            MaxGPA = 3.70m,
            Count = 1,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            AcademicYear = 2003,
            SemesterNumber = 1,
            AverageGPA = 3.700000m,
            MinGPA = 3.70m,
            MaxGPA = 3.70m,
            Count = 1,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            AcademicYear = 2004,
            SemesterNumber = 1,
            AverageGPA = 4.000000m,
            MinGPA = 4.00m,
            MaxGPA = 4.00m,
            Count = 1,
        },
        new()
        {
            StudentId = new("562f5b34-7e6f-4738-a5fe-1cac5d0c09f3"),
            AcademicYear = 2004,
            SemesterNumber = 2,
            AverageGPA = 3.700000m,
            MinGPA = 3.70m,
            MaxGPA = 3.70m,
            Count = 1,
        },
        new()
        {
            StudentId = new("907068da-f84b-4a03-b982-5fc7e7137c7b"),
            AcademicYear = 2006,
            SemesterNumber = 2,
            AverageGPA = 4.000000m,
            MinGPA = 4.00m,
            MaxGPA = 4.00m,
            Count = 1,
        },
        new()
        {
            StudentId = new("907068da-f84b-4a03-b982-5fc7e7137c7b"),
            AcademicYear = 2008,
            SemesterNumber = 1,
            AverageGPA = 3.700000m,
            MinGPA = 3.70m,
            MaxGPA = 3.70m,
            Count = 1,
        },
        new()
        {
            StudentId = new("b05f9f2f-38c0-4067-afd1-c961be997fdd"),
            AcademicYear = 2001,
            SemesterNumber = 2,
            AverageGPA = 3.700000m,
            MinGPA = 3.70m,
            MaxGPA = 3.70m,
            Count = 1,
        },
        new()
        {
            StudentId = new("b05f9f2f-38c0-4067-afd1-c961be997fdd"),
            AcademicYear = 2002,
            SemesterNumber = 2,
            AverageGPA = 4.000000m,
            MinGPA = 4.00m,
            MaxGPA = 4.00m,
            Count = 1,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            AcademicYear = 2000,
            SemesterNumber = 3,
            AverageGPA = 3.700000m,
            MinGPA = 3.70m,
            MaxGPA = 3.70m,
            Count = 1,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            AcademicYear = 2001,
            SemesterNumber = 1,
            AverageGPA = 3.650000m,
            MinGPA = 3.30m,
            MaxGPA = 4.00m,
            Count = 2,
        },
        new()
        {
            StudentId = new("b263f8eb-032c-4f7b-94a0-3360cc37a827"),
            AcademicYear = 2001,
            SemesterNumber = 3,
            AverageGPA = 4.000000m,
            MinGPA = 4.00m,
            MaxGPA = 4.00m,
            Count = 1,
        },
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            AcademicYear = 2006,
            SemesterNumber = 2,
            AverageGPA = 4.000000m,
            MinGPA = 4.00m,
            MaxGPA = 4.00m,
            Count = 1,
        },
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            AcademicYear = 2007,
            SemesterNumber = 3,
            AverageGPA = 3.700000m,
            MinGPA = 3.70m,
            MaxGPA = 3.70m,
            Count = 1,
        },
        new()
        {
            StudentId = new("c4d881ad-111e-45a0-82a8-ae798b838a06"),
            AcademicYear = 2008,
            SemesterNumber = 1,
            AverageGPA = 4.000000m,
            MinGPA = 4.00m,
            MaxGPA = 4.00m,
            Count = 1,
        },
        new()
        {
            StudentId = new("cf528c9a-557c-40ff-ad70-5647b0c7ec2a"),
            AcademicYear = 2000,
            SemesterNumber = 3,
            AverageGPA = 3.700000m,
            MinGPA = 3.70m,
            MaxGPA = 3.70m,
            Count = 1,
        },
        new()
        {
            StudentId = new("ff6f3449-8218-40b0-9914-7da3bb22f179"),
            AcademicYear = 2006,
            SemesterNumber = 1,
            AverageGPA = 3.650000m,
            MinGPA = 3.30m,
            MaxGPA = 4.00m,
            Count = 2,
        },
    ];
}
